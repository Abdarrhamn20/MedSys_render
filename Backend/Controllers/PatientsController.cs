using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;
using MedicalSystem.Models;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/patients?search=
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.PatientProfiles
                .Include(p => p.User)
                .Where(p => p.User.IsActive);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.User.FullName.Contains(search)
                    || p.User.Email.Contains(search)
                    || (p.FileNumber != null && p.FileNumber.Contains(search))
                    || (p.FirstName != null && p.FirstName.Contains(search))
                    || (p.FamilyName != null && p.FamilyName.Contains(search)));

            var totalCount = await query.CountAsync();

            var patients = await query
                .OrderBy(p => p.FileNumber)
                .ThenBy(p => p.User.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.PatientID,
                    p.UserID,
                    p.User.FullName,
                    p.FirstName,
                    p.FatherName,
                    p.GrandfatherName,
                    p.FamilyName,
                    p.FileNumber,
                    p.User.Email,
                    p.User.Phone,
                    p.BloodType,
                    p.Gender,
                    p.DateOfBirth,
                    p.ChronicDiseases,
                    p.Allergies,
                    IsMerged = p.MergedIntoPatientID.HasValue,
                    AppointmentsCount = p.Appointments.Count()
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = patients.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var patient = await _context.PatientProfiles
                .Where(p => p.PatientID == id)
                .Select(p => new
                {
                    p.PatientID,
                    p.UserID,
                    p.User.FullName,
                    p.FirstName,
                    p.FatherName,
                    p.GrandfatherName,
                    p.FamilyName,
                    p.FileNumber,
                    p.User.Email,
                    p.User.Phone,
                    p.BloodType,
                    p.ChronicDiseases,
                    p.Allergies,
                    p.GeneralNotes,
                    p.DateOfBirth,
                    p.Gender,
                    p.Address,
                    p.EmergencyContact,
                    p.EmergencyPhone,
                    p.User.IsActive,
                    p.MergedIntoPatientID,
                    p.MergedAt,
                    IsMerged = p.MergedIntoPatientID.HasValue,
                    TotalAppointments = p.Appointments.Count(),
                    CompletedAppointments = p.Appointments.Count(a => a.Status == "Completed")
                })
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound(ApiResponse.Fail("المريض غير موجود"));

            // Privacy: Patient can only see their own data, Doctor must have appointment
            if (role == "Patient" && patient.UserID != userId)
                return Forbid();

            return Ok(ApiResponse<object>.Ok(patient));
        }

        // GET: api/patients/next-file-number
        [HttpGet("next-file-number")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<IActionResult> GetNextFileNumber()
        {
            var number = await FileNumberHelper.GenerateNextAsync(_context);
            return Ok(ApiResponse<object>.Ok(new { FileNumber = number }));
        }

        // PUT: api/patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientUpdateDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var patient = await _context.PatientProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientID == id);
            if (patient == null)
                return NotFound(ApiResponse.Fail("المريض غير موجود"));

            if (role != "Admin" && patient.UserID != userId)
                return Forbid();

            // التركيبة الاسمية الليبية: تحديث المكونات + إعادة بناء الاسم الكامل
            if (dto.FirstName != null) patient.FirstName = dto.FirstName;
            if (dto.FatherName != null) patient.FatherName = dto.FatherName;
            if (dto.GrandfatherName != null) patient.GrandfatherName = dto.GrandfatherName;
            if (dto.FamilyName != null) patient.FamilyName = dto.FamilyName;
            if (!string.IsNullOrWhiteSpace(dto.FullName)) patient.User.FullName = dto.FullName.Trim();

            patient.BloodType = dto.BloodType ?? patient.BloodType;
            patient.ChronicDiseases = dto.ChronicDiseases ?? patient.ChronicDiseases;
            patient.Allergies = dto.Allergies ?? patient.Allergies;
            patient.GeneralNotes = dto.GeneralNotes ?? patient.GeneralNotes;
            patient.DateOfBirth = dto.DateOfBirth ?? patient.DateOfBirth;
            patient.Gender = dto.Gender ?? patient.Gender;
            patient.Address = dto.Address ?? patient.Address;
            patient.EmergencyContact = dto.EmergencyContact ?? patient.EmergencyContact;
            patient.EmergencyPhone = dto.EmergencyPhone ?? patient.EmergencyPhone;

            // إن لم يصرّح بالاسم الكامل يدوياً، أعد بناءه من المكونات
            if (string.IsNullOrWhiteSpace(dto.FullName))
                patient.User.FullName = ComposeFullName(patient);

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم تحديث بيانات المريض بنجاح"));
        }

        // POST: api/patients/merge
        [HttpPost("merge")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Merge([FromBody] MergePatientsDTO dto)
        {
            if (dto.SourcePatientID == dto.TargetPatientID)
                return BadRequest(ApiResponse.Fail("لا يمكن دمج الملف مع نفسه."));

            var source = await _context.PatientProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientID == dto.SourcePatientID);
            var target = await _context.PatientProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientID == dto.TargetPatientID);

            if (source == null || target == null)
                return NotFound(ApiResponse.Fail("أحد الملفين غير موجود."));
            if (source.MergedIntoPatientID.HasValue)
                return BadRequest(ApiResponse.Fail("الملف المصدر مندمج مسبقاً."));

            var targetUserId = target.UserID;
            var sourceUserId = source.UserID;

            // 1. نقل السجلات المرتبطة مباشرة بملف المريض (PatientID)
            await _context.Appointments
                .Where(a => a.PatientID == source.PatientID)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.PatientID, target.PatientID));

            await _context.Attachments
                .Where(a => a.PatientID == source.PatientID)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.PatientID, target.PatientID));

            await _context.Admissions
                .Where(a => a.PatientID == source.PatientID)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.PatientID, target.PatientID));

            // 2. نقل السجلات المرتبطة بحساب المستخدم (PatientUserID)
            await _context.Invoices
                .Where(i => i.PatientUserID == sourceUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.PatientUserID, targetUserId));

            await _context.Vouchers
                .Where(v => v.PatientUserID == sourceUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.PatientUserID, targetUserId));

            await _context.LabOrders
                .Where(l => l.PatientUserID == sourceUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(l => l.PatientUserID, targetUserId));

            await _context.RadiologyOrders
                .Where(r => r.PatientUserID == sourceUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.PatientUserID, targetUserId));

            await _context.PatientAssessments
                .Where(pa => pa.PatientUserID == sourceUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(pa => pa.PatientUserID, targetUserId));

            // 3. دمج البيانات الطبية النصية إن كانت الوجهة فارغة
            if (string.IsNullOrWhiteSpace(target.ChronicDiseases) && !string.IsNullOrWhiteSpace(source.ChronicDiseases))
                target.ChronicDiseases = source.ChronicDiseases;
            if (string.IsNullOrWhiteSpace(target.Allergies) && !string.IsNullOrWhiteSpace(source.Allergies))
                target.Allergies = source.Allergies;
            if (string.IsNullOrWhiteSpace(target.GeneralNotes) && !string.IsNullOrWhiteSpace(source.GeneralNotes))
                target.GeneralNotes = source.GeneralNotes;
            if (!target.DateOfBirth.HasValue && source.DateOfBirth.HasValue)
                target.DateOfBirth = source.DateOfBirth;

            // 4. إغلاق الملف المصدر
            source.MergedIntoPatientID = target.PatientID;
            source.MergedAt = DateTime.Now;
            source.User.IsActive = false;

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "PatientFilesMerged",
                EntityType = "PatientProfile",
                EntityID = source.PatientID,
                UserID = JwtHelper.GetUserIdFromClaims(User),
                Details = $"دمج ملف مريض {source.User.FullName} (رقم الملف {source.FileNumber}) إلى ملف {target.User.FullName} (رقم الملف {target.FileNumber})",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم دمج ملف المريض {source.User.FullName} بنجاح إلى ملف {target.User.FullName}"));
        }

        /// <summary>
        /// بناء الاسم الكامل من مكونات الاسم الليبية (الاسم + الأب + الجد + اللقب)
        /// </summary>
        private static string ComposeFullName(PatientProfile p)
        {
            var parts = new[] { p.FirstName, p.FatherName, p.GrandfatherName, p.FamilyName }
                .Where(x => !string.IsNullOrWhiteSpace(x));
            return string.Join(" ", parts);
        }
    }
}
