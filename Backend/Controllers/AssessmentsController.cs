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
    public class AssessmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssessmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        //  TEMPLATES (قوالب الاستبيانات)
        // ==========================================

        // GET: api/assessments/templates
        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.CustomAssessmentTemplates.Where(t => t.IsActive).AsQueryable();

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                // الطبيب يرى القوالب العامة + قوالبه الخاصة
                query = query.Where(t => t.DoctorID == null || t.DoctorID == doctorId);
            }

            var templates = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.TemplateID,
                    t.DoctorID,
                    t.Title,
                    t.Description,
                    t.SchemaJson,
                    t.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(templates));
        }

        // POST: api/assessments/templates
        [HttpPost("templates")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            int? doctorId = null;
            if (role == "Doctor")
            {
                doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
            }

            var template = new CustomAssessmentTemplate
            {
                DoctorID = doctorId,
                Title = dto.Title,
                Description = dto.Description,
                SchemaJson = dto.SchemaJson,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.CustomAssessmentTemplates.Add(template);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(template, "تم إنشاء قالب الاستبيان بنجاح"));
        }

        // DELETE: api/assessments/templates/5
        [HttpDelete("templates/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var template = await _context.CustomAssessmentTemplates.FindAsync(id);
            if (template == null)
                return NotFound(ApiResponse.Fail("قالب الاستبيان غير موجود"));

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (template.DoctorID != doctorId)
                    return Forbid();
            }

            template.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف قالب الاستبيان بنجاح"));
        }

        // ==========================================
        //  ASSIGNMENTS (إسناد الاستبيانات للمرضى)
        // ==========================================

        // POST: api/assessments/assign
        [HttpPost("assign")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AssignAssessment([FromBody] AssignAssessmentDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();

            var template = await _context.CustomAssessmentTemplates.FindAsync(dto.TemplateID);
            if (template == null || !template.IsActive)
                return NotFound(ApiResponse.Fail("قالب الاستبيان غير موجود أو غير نشط"));

            // تحقق من وجود المريض
            var patientExists = await _context.PatientProfiles.AnyAsync(p => p.UserID == dto.PatientUserID);
            if (!patientExists)
                return NotFound(ApiResponse.Fail("الملف الشخصي للمريض غير موجود"));

            // منع إسناد مكرر بانتظار التعبئة لنفس المريض والقالب
            var alreadyAssigned = await _context.PatientAssessments.AnyAsync(pa =>
                pa.PatientUserID == dto.PatientUserID && pa.TemplateID == dto.TemplateID && pa.Status == "Pending");
            if (alreadyAssigned)
                return BadRequest(ApiResponse.Fail("هذا الاستبيان مُسند للمريض مسبقاً بانتظار التعبئة"));

            var assessment = new PatientAssessment
            {
                PatientUserID = dto.PatientUserID,
                TemplateID = dto.TemplateID,
                AnswersJson = "{}",
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.PatientAssessments.Add(assessment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(assessment, "تم إسناد وطلب تعبئة الاستبيان من المريض بنجاح"));
        }

        // ==========================================
        //  PATIENT FLOW (واجهات المريض للتعبئة)
        // ==========================================

        // GET: api/assessments/patient/pending
        [HttpGet("patient/pending")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> GetPatientPending()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.PatientAssessments
                .Include(pa => pa.CustomAssessmentTemplate)
                .Where(pa => pa.Status == "Pending");

            if (role == "Patient")
            {
                query = query.Where(pa => pa.PatientUserID == userId);
            }
            else if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                query = query.Where(pa => pa.CustomAssessmentTemplate.DoctorID == null || pa.CustomAssessmentTemplate.DoctorID == doctorId);
            }

            var pending = await query
                .OrderByDescending(pa => pa.CreatedAt)
                .Select(pa => new
                {
                    pa.AssessmentID,
                    pa.TemplateID,
                    TemplateTitle = pa.CustomAssessmentTemplate.Title,
                    TemplateDescription = pa.CustomAssessmentTemplate.Description,
                    pa.CustomAssessmentTemplate.SchemaJson,
                    pa.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(pending));
        }

        // GET: api/assessments/patient/completed
        [HttpGet("patient/completed")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> GetPatientCompleted()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.PatientAssessments
                .Include(pa => pa.CustomAssessmentTemplate)
                .Where(pa => pa.Status == "Completed");

            if (role == "Patient")
            {
                query = query.Where(pa => pa.PatientUserID == userId);
            }
            else if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                query = query.Where(pa => pa.CustomAssessmentTemplate.DoctorID == null || pa.CustomAssessmentTemplate.DoctorID == doctorId);
            }

            var completed = await query
                .OrderByDescending(pa => pa.CompletedAt)
                .Select(pa => new
                {
                    pa.AssessmentID,
                    pa.TemplateID,
                    TemplateTitle = pa.CustomAssessmentTemplate.Title,
                    TemplateDescription = pa.CustomAssessmentTemplate.Description,
                    pa.AnswersJson,
                    pa.CreatedAt,
                    pa.CompletedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(completed));
        }

        // POST: api/assessments/patient/submit/5
        [HttpPost("patient/submit/{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SubmitAnswers(int id, [FromBody] SubmitAnswersDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            var assessment = await _context.PatientAssessments
                .FirstOrDefaultAsync(pa => pa.AssessmentID == id && pa.PatientUserID == userId);

            if (assessment == null)
                return NotFound(ApiResponse.Fail("طلب الاستبيان غير موجود أو لا يخصك"));

            if (assessment.Status == "Completed")
                return BadRequest(ApiResponse.Fail("تم إرسال هذا الاستبيان مسبقاً"));

            assessment.AnswersJson = dto.AnswersJson;
            assessment.Status = "Completed";
            assessment.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم إرسال إجابات الاستبيان بنجاح"));
        }

        // ==========================================
        //  DOCTOR RESULTS (عرض النتائج للطبيب)
        // ==========================================

        // GET: api/assessments/results/5
        [HttpGet("results/{id}")]
        public async Task<IActionResult> GetAssessmentResults(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var assessment = await _context.PatientAssessments
                .Include(pa => pa.CustomAssessmentTemplate)
                .Include(pa => pa.PatientUser)
                .FirstOrDefaultAsync(pa => pa.AssessmentID == id);

            if (assessment == null)
                return NotFound(ApiResponse.Fail("الاستبيان غير موجود"));

            // الصلاحيات: المريض صاحب الاستبيان، أو طبيب قالب الإسناد، أو الأدمن، وغيرهم مرفوضون
            if (role == "Patient")
            {
                if (assessment.PatientUserID != userId)
                    return Forbid();
            }
            else if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (assessment.CustomAssessmentTemplate == null ||
                    (assessment.CustomAssessmentTemplate.DoctorID != null && assessment.CustomAssessmentTemplate.DoctorID != doctorId))
                    return Forbid();
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            var result = new
            {
                assessment.AssessmentID,
                assessment.TemplateID,
                TemplateTitle = assessment.CustomAssessmentTemplate.Title,
                TemplateDescription = assessment.CustomAssessmentTemplate.Description,
                TemplateSchema = assessment.CustomAssessmentTemplate.SchemaJson,
                PatientName = assessment.PatientUser.FullName,
                assessment.AnswersJson,
                assessment.Status,
                assessment.CreatedAt,
                assessment.CompletedAt
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // GET: api/assessments/patient-list/5
        [HttpGet("patient-list/{patientUserId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetPatientAssessments(int patientUserId)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.PatientAssessments
                .Include(pa => pa.CustomAssessmentTemplate)
                .Where(pa => pa.PatientUserID == patientUserId)
                .AsQueryable();

            // الطبيب يرى تقييمات مرضاه التي أُنشئت من قوالبه أو القوالب العامة فقط
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                query = query.Where(pa => pa.CustomAssessmentTemplate.DoctorID == null || pa.CustomAssessmentTemplate.DoctorID == doctorId);
            }

            var assessments = await query
                .OrderByDescending(pa => pa.CreatedAt)
                .Select(pa => new
                {
                    pa.AssessmentID,
                    pa.TemplateID,
                    TemplateTitle = pa.CustomAssessmentTemplate.Title,
                    pa.Status,
                    pa.CreatedAt,
                    pa.CompletedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(assessments));
        }

        // GET: api/assessments/stats/pending-count
        [HttpGet("stats/pending-count")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetPendingCount()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            int count;
            if (role == "Admin")
            {
                count = await _context.PatientAssessments.CountAsync(pa => pa.Status == "Pending");
            }
            else
            {
                var doctorId = await _context.DoctorProfiles
                    .Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();

                // Count pending assessments from templates created by this doctor
                count = await _context.PatientAssessments
                    .Include(pa => pa.CustomAssessmentTemplate)
                    .CountAsync(pa => pa.Status == "Pending" &&
                        (pa.CustomAssessmentTemplate.DoctorID == doctorId || pa.CustomAssessmentTemplate.DoctorID == null));
            }

            return Ok(ApiResponse<object>.Ok(count));
        }
    }
}
