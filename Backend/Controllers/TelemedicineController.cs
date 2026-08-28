using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Models;
using MedicalSystem.Helpers;
using MedicalSystem.Services;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TelemedicineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppNotificationService _notificationService;

        public TelemedicineController(ApplicationDbContext context, IAppNotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: api/telemedicine/sessions/{appointmentId}
        [HttpGet("sessions/{appointmentId}")]
        public async Task<IActionResult> GetSession(int appointmentId)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var appointment = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppID == appointmentId);

            if (appointment == null)
                return NotFound(ApiResponse.Fail("الموعد غير موجود"));

            if (!await IsParticipantAsync(appointment, userId, role))
                return Forbid();

            var session = await _context.TelemedicineSessions
                .Where(s => s.AppointmentID == appointmentId && s.Status != "Ended")
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (session == null)
                return Ok(ApiResponse<object>.Ok((object)null!, "لا توجد جلسة فيديو نشطة لهذا الموعد"));

            return Ok(ApiResponse<object>.Ok(MapToDTO(session)));
        }

        // POST: api/telemedicine/sessions
        [HttpPost("sessions")]
        public async Task<IActionResult> CreateOrGetSession([FromBody] CreateTelemedicineSessionDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var appointment = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppID == dto.AppointmentID);

            if (appointment == null)
                return NotFound(ApiResponse.Fail("الموعد غير موجود"));

            if (!await IsParticipantAsync(appointment, userId, role))
                return Forbid();

            // الجلسة متاحة فقط للمواعيد المؤكدة أو الجارية
            if (appointment.Status != "Confirmed" && appointment.Status != "InProgress")
                return BadRequest(ApiResponse.Fail("لا يمكن بدء جلسة فيديو إلا لموعد مؤكد أو جارٍ"));

            // أعد الجلسة النشطة إن وُجدت بدلاً من إنشاء أخرى
            var existing = await _context.TelemedicineSessions
                .Where(s => s.AppointmentID == dto.AppointmentID && s.Status != "Ended")
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (existing != null)
                return Ok(ApiResponse<object>.Ok(MapToDTO(existing), "تم الانضمام إلى الجلسة النشطة"));

            var session = new TelemedicineSession
            {
                AppointmentID = dto.AppointmentID,
                RoomCode = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                Status = "Waiting",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now,
                SessionNotes = dto.SessionNotes
            };

            _context.TelemedicineSessions.Add(session);

            // إشعار المريض بأن الطبيب فتح معه مكالمة الفيديو (داخل النظام + Push)
            if (appointment.Patient?.UserID != null)
            {
                var doctorName = appointment.Doctor?.User?.FullName ?? "الطبيب";
                await _notificationService.SendInAppAndPushAsync(
                    _context,
                    appointment.Patient.UserID,
                    "مكالمة فيديو جاهزة 📹",
                    $"الطبيب د. {doctorName} فتح معك جلسة فيديو — انضم الآن إلى الجلسة في انتظارك.",
                    "TelemedicineStarted",
                    "Appointment",
                    appointment.AppID);
            }
            else
            {
                await _context.SaveChangesAsync();
            }

            // تحديث حالة الموعد إلى جارية عند بدء الجلسة
            if (appointment.Status == "Confirmed")
            {
                appointment.Status = "InProgress";
                _context.AuditLogs.Add(new AuditLog
                {
                    ActionType = "TelemedicineStarted",
                    EntityType = "Appointment",
                    EntityID = appointment.AppID,
                    UserID = userId,
                    Details = $"تم بدء جلسة فيديو عن بعد للموعد #{appointment.AppID}",
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse<object>.Ok(MapToDTO(session), "تم إنشاء جلسة الفيديو بنجاح"));
        }

        // GET: api/telemedicine/sessions/{appointmentId}/history
        [HttpGet("sessions/{appointmentId}/history")]
        public async Task<IActionResult> GetSessionHistory(int appointmentId)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var appointment = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppID == appointmentId);

            if (appointment == null)
                return NotFound(ApiResponse.Fail("الموعد غير موجود"));

            if (!await IsParticipantAsync(appointment, userId, role))
                return Forbid();

            var sessions = await _context.TelemedicineSessions
                .Where(s => s.AppointmentID == appointmentId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new TelemedicineSessionDTO
                {
                    SessionID = s.SessionID,
                    AppointmentID = s.AppointmentID,
                    RoomCode = s.RoomCode,
                    Status = s.Status,
                    CreatedByUserID = s.CreatedByUserID,
                    CreatedAt = s.CreatedAt,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt,
                    SessionNotes = s.SessionNotes,
                    PatientName = appointment.Patient.User.FullName,
                    DoctorName = appointment.Doctor.User.FullName,
                    DoctorSpecialty = appointment.Doctor.Specialty,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    AppointmentStatus = appointment.Status
                })
                .ToListAsync();

            return Ok(ApiResponse<List<TelemedicineSessionDTO>>.Ok(sessions, "سجل جلسات الفيديو"));
        }

        // POST: api/telemedicine/sessions/{id}/start
        [HttpPost("sessions/{id}/start")]
        public async Task<IActionResult> StartSession(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var session = await _context.TelemedicineSessions
                .Include(s => s.Appointment)
                .FirstOrDefaultAsync(s => s.SessionID == id);

            if (session == null)
                return NotFound(ApiResponse.Fail("الجلسة غير موجودة"));

            if (session.Appointment == null)
                return NotFound(ApiResponse.Fail("الموعد المرتبط بالجلسة غير موجود"));

            // لا يبدأ الجلسة إلا طرف في الموعد (كانت قابلة للبدء من أي مستخدم)
            if (!await IsParticipantAsync(session.Appointment, userId, role))
                return Forbid();

            if (session.Status == "Ended")
                return BadRequest(ApiResponse.Fail("انتهت الجلسة"));

            if (session.Status != "Active")
            {
                session.Status = "Active";
                session.StartedAt = DateTime.Now;
            }

            session.Appointment.Status = "InProgress";
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(MapToDTO(session), "بدأت الجلسة"));
        }

        // POST: api/telemedicine/sessions/{id}/end
        [HttpPost("sessions/{id}/end")]
        public async Task<IActionResult> EndSession(int id, [FromBody] EndTelemedicineSessionDTO? dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            var session = await _context.TelemedicineSessions
                .Include(s => s.Appointment)
                .FirstOrDefaultAsync(s => s.SessionID == id);

            if (session == null)
                return NotFound(ApiResponse.Fail("الجلسة غير موجودة"));

            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (!await IsParticipantAsync(session.Appointment, userId, role))
                return Forbid();

            if (session.Status != "Ended")
            {
                session.Status = "Ended";
                session.EndedAt = DateTime.Now;

                _context.AuditLogs.Add(new AuditLog
                {
                    ActionType = "TelemedicineEnded",
                    EntityType = "TelemedicineSession",
                    EntityID = session.SessionID,
                    UserID = userId,
                    Details = $"انتهت جلسة الفيديو للموعد #{session.AppointmentID}",
                    Timestamp = DateTime.Now
                });
            }

            // تُحدَّث الملاحظات حتى بعد إنهاء الجلسة (حفظ ملاحظات الطبيب بعد المكالمة)
            if (dto != null && dto.SessionNotes != null)
                session.SessionNotes = dto.SessionNotes;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(MapToDTO(session), "تم إنهاء الجلسة"));
        }

        private async Task<bool> IsParticipantAsync(Appointment appointment, int userId, string role)
        {
            if (role == "Admin")
                return true;

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                return appointment.DoctorID == doctorId;
            }

            if (role == "Patient")
            {
                var patientId = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                return appointment.PatientID == patientId;
            }

            return false;
        }

        private object MapToDTO(TelemedicineSession session)
        {
            return new TelemedicineSessionDTO
            {
                SessionID = session.SessionID,
                AppointmentID = session.AppointmentID,
                RoomCode = session.RoomCode,
                Status = session.Status,
                CreatedByUserID = session.CreatedByUserID,
                CreatedAt = session.CreatedAt,
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt,
                SessionNotes = session.SessionNotes,
                PatientName = session.Appointment?.Patient?.User?.FullName,
                DoctorName = session.Appointment?.Doctor?.User?.FullName,
                DoctorSpecialty = session.Appointment?.Doctor?.Specialty,
                AppointmentDate = session.Appointment?.AppointmentDate,
                AppointmentTime = session.Appointment?.AppointmentTime,
                AppointmentStatus = session.Appointment?.Status
            };
        }
    }
}
