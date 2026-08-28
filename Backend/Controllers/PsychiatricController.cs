using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Models;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PsychiatricController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PsychiatricController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/psychiatric/records/5
        [HttpGet("records/{recordId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetPsychiatricRecord(int recordId)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var record = await _context.MedicalRecords
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.RecordID == recordId);

            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي الأساسي غير موجود"));

            // Verify permission (Doctor must own the appointment, Admin can see all)
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (record.Appointment.DoctorID != doctorId)
                    return Forbid();
            }

            var psychRecord = await _context.PsychiatricRecords
                .FirstOrDefaultAsync(p => p.RecordID == recordId);

            // If not exists, return empty object with recordId
            if (psychRecord == null)
            {
                return Ok(ApiResponse<object>.Ok(new
                {
                    RecordID = recordId,
                    Appearance = "",
                    Behavior = "",
                    Speech = "",
                    MoodAndAffect = "",
                    ThoughtProcess = "",
                    ThoughtContent = "",
                    Perception = "",
                    Cognition = "",
                    InsightAndJudgment = "",
                    IsSpeechToTextUsed = false
                }, "لم يتم إنشاء سجل نفسي بعد، تم إرجاع نموذج فارغ"));
            }

            return Ok(ApiResponse<object>.Ok(psychRecord));
        }

        // POST: api/psychiatric/records/5
        [HttpPost("records/{recordId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SavePsychiatricRecord(int recordId, [FromBody] SavePsychiatricRecordDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            var record = await _context.MedicalRecords
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.RecordID == recordId);

            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي الأساسي غير موجود"));

            // Verify permission
            var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
            if (record.Appointment.DoctorID != doctorId)
                return Forbid();

            var psychRecord = await _context.PsychiatricRecords
                .FirstOrDefaultAsync(p => p.RecordID == recordId);

            bool isNew = false;
            if (psychRecord == null)
            {
                isNew = true;
                psychRecord = new PsychiatricRecord
                {
                    RecordID = recordId
                };
            }

            psychRecord.Appearance = dto.Appearance;
            psychRecord.Behavior = dto.Behavior;
            psychRecord.Speech = dto.Speech;
            psychRecord.MoodAndAffect = dto.MoodAndAffect;
            psychRecord.ThoughtProcess = dto.ThoughtProcess;
            psychRecord.ThoughtContent = dto.ThoughtContent;
            psychRecord.Perception = dto.Perception;
            psychRecord.Cognition = dto.Cognition;
            psychRecord.InsightAndJudgment = dto.InsightAndJudgment;
            psychRecord.IsSpeechToTextUsed = dto.IsSpeechToTextUsed;
            psychRecord.CreatedAt = DateTime.Now;

            if (isNew)
            {
                _context.PsychiatricRecords.Add(psychRecord);
            }
            else
            {
                _context.PsychiatricRecords.Update(psychRecord);
            }

            // Add Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = isNew ? "CreatePsychiatricRecord" : "UpdatePsychiatricRecord",
                EntityType = "PsychiatricRecord",
                EntityID = recordId,
                UserID = userId,
                Details = $"{(isNew ? "إنشاء" : "تعديل")} سجل فحص الحالة العقلية (MSE) التابع للسجل الطبي #{recordId}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(psychRecord, "تم حفظ سجل فحص الحالة العقلية (MSE) بنجاح"));
        }

        // ==========================================
        //  SOAP NOTES ENDPOINTS
        // ==========================================

        // GET: api/psychiatric/soap/5
        [HttpGet("soap/{recordId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetSoapNote(int recordId)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var record = await _context.MedicalRecords
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.RecordID == recordId);

            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي الأساسي غير موجود"));

            // Verify permission (Doctor must own the appointment)
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (record.Appointment.DoctorID != doctorId)
                    return Forbid();
            }

            var soap = await _context.SoapNotes
                .FirstOrDefaultAsync(s => s.RecordID == recordId);

            // If not exists, return empty object with recordId
            if (soap == null)
            {
                return Ok(ApiResponse<object>.Ok(new
                {
                    soapNoteID = 0,
                    recordID = recordId,
                    subjective = "",
                    objective = "",
                    assessment = "",
                    plan = "",
                    createdAt = DateTime.Now
                }, "لم يتم إنشاء SOAP Note بعد، تم إرجاع نموذج فارغ"));
            }

            return Ok(ApiResponse<object>.Ok(soap));
        }

        // POST: api/psychiatric/soap/5
        [HttpPost("soap/{recordId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SaveSoapNote(int recordId, [FromBody] SaveSoapNoteDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            var record = await _context.MedicalRecords
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.RecordID == recordId);

            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي الأساسي غير موجود"));

            // Verify permission
            var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
            if (record.Appointment.DoctorID != doctorId)
                return Forbid();

            var soap = await _context.SoapNotes
                .FirstOrDefaultAsync(s => s.RecordID == recordId);

            bool isNew = false;
            if (soap == null)
            {
                isNew = true;
                soap = new SoapNote
                {
                    RecordID = recordId
                };
            }

            soap.Subjective = dto.Subjective;
            soap.Objective = dto.Objective;
            soap.Assessment = dto.Assessment;
            soap.Plan = dto.Plan;
            soap.UpdatedAt = DateTime.Now;

            if (isNew)
            {
                _context.SoapNotes.Add(soap);
            }
            else
            {
                _context.SoapNotes.Update(soap);
            }

            // Add Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = isNew ? "CreateSoapNote" : "UpdateSoapNote",
                EntityType = "SoapNote",
                EntityID = recordId,
                UserID = userId,
                Details = $"{(isNew ? "إنشاء" : "تعديل")} سجل SOAP Note للسجل الطبي #{recordId}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(soap, "تم حفظ سجل SOAP Note بنجاح"));
        }

        // ==========================================
        //  RISK LEVEL ENDPOINT (للمرضى النفسيين)
        // ==========================================

        // PUT: api/psychiatric/patient-risk/5  (5 = PatientID)
        [HttpPut("patient-risk/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateRiskLevel(int patientId, [FromBody] UpdateRiskLevelDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            var patient = await _context.PatientProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientID == patientId);

            if (patient == null)
                return NotFound(ApiResponse.Fail("الملف الشخصي للمريض غير موجود"));

            // Validate risk level value
            var validLevels = new[] { "Stable", "Monitoring", "Critical" };
            if (!validLevels.Contains(dto.RiskLevel))
                return BadRequest(ApiResponse.Fail("قيمة مستوى الخطورة غير صالحة. يجب أن تكون: Stable أو Monitoring أو Critical"));

            patient.RiskLevel = dto.RiskLevel;
            patient.RiskLevelUpdatedAt = DateTime.Now;
            patient.RiskLevelUpdatedByUserID = userId;
            patient.RiskLevelNotes = dto.Notes;

            // Add Audit Log
            var levelAr = dto.RiskLevel == "Stable" ? "مستقر 🟢" : dto.RiskLevel == "Monitoring" ? "تحت الملاحظة 🟡" : "حرج/خطر إيذاء النفس 🔴";
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "UpdateRiskLevel",
                EntityType = "PatientProfile",
                EntityID = patientId,
                UserID = userId,
                Details = $"تحديث مستوى الخطورة للمريض {patient.User?.FullName} إلى: {levelAr}" + (dto.Notes != null ? $" — ملاحظات: {dto.Notes}" : ""),
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                patientId = patient.PatientID,
                riskLevel = patient.RiskLevel,
                riskLevelAr = levelAr,
                riskLevelUpdatedAt = patient.RiskLevelUpdatedAt,
                riskLevelNotes = patient.RiskLevelNotes
            }, "تم تحديث مستوى الخطورة السريرية للمريض بنجاح"));
        }

        // GET: api/psychiatric/patient-risk/5  (5 = PatientID)
        [HttpGet("patient-risk/{patientId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetRiskLevel(int patientId)
        {
            var patient = await _context.PatientProfiles
                .FirstOrDefaultAsync(p => p.PatientID == patientId);

            if (patient == null)
                return NotFound(ApiResponse.Fail("الملف الشخصي للمريض غير موجود"));

            var levelAr = patient.RiskLevel switch
            {
                "Monitoring" => "تحت الملاحظة 🟡",
                "Critical" => "حرج/خطر إيذاء النفس 🔴",
                _ => "مستقر 🟢"
            };

            return Ok(ApiResponse<object>.Ok(new
            {
                patientId = patient.PatientID,
                riskLevel = patient.RiskLevel ?? "Stable",
                riskLevelAr = levelAr,
                riskLevelUpdatedAt = patient.RiskLevelUpdatedAt,
                riskLevelNotes = patient.RiskLevelNotes
            }));
        }
    }
}
