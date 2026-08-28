using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.Models;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AttachmentsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: api/attachments/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachment([FromForm] IFormFile file, [FromForm] int? recordId, [FromForm] int? patientId, [FromForm] string? description)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse.Fail("لم يتم تحديد أي ملف."));

            // Validation
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(ApiResponse.Fail("نوع الملف غير مدعوم. مسموح فقط بـ: JPG, PNG, PDF."));

            if (file.Length > 5 * 1024 * 1024) // 5 MB max
                return BadRequest(ApiResponse.Fail("حجم الملف يجب ألا يتجاوز 5 ميجابايت."));

            if (recordId == null && patientId == null)
                return BadRequest(ApiResponse.Fail("يجب ربط المرفق بسجل طبي أو مريض."));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (recordId.HasValue)
            {
                var record = await _context.MedicalRecords.Include(m => m.Appointment).FirstOrDefaultAsync(m => m.RecordID == recordId.Value);
                if (record == null)
                    return NotFound(ApiResponse.Fail("السجل الطبي غير موجود."));
                if (role == "Doctor")
                {
                    var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                    if (record.Appointment.DoctorID != doctorId)
                        return Forbid();
                }
                else if (role == "Patient")
                {
                    var patientIdOwn = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                    if (record.Appointment.PatientID != patientIdOwn)
                        return Forbid();
                }
                else if (role != "Admin")
                {
                    return Forbid();
                }
            }
            else if (patientId.HasValue)
            {
                var patient = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.PatientID == patientId.Value);
                if (patient == null)
                    return NotFound(ApiResponse.Fail("الملف الطبي للمريض غير موجود."));
                if (role == "Patient")
                {
                    var patientIdOwn = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                    if (patientId.Value != patientIdOwn)
                        return Forbid();
                }
                else if (role == "Doctor")
                {
                    var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                    var hasRelationship = await _context.Appointments.AnyAsync(a => a.PatientID == patientId.Value && a.DoctorID == doctorId);
                    if (!hasRelationship)
                        return Forbid();
                }
                else if (role != "Admin")
                {
                    return Forbid();
                }
            }

            // Create Directory if not exists
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "attachments");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save metadata to database
            var attachment = new Attachment
            {
                FileName = file.FileName, // Original name
                FileType = file.ContentType,
                FileURL = $"/uploads/attachments/{uniqueFileName}",
                FileSize = file.Length,
                Description = description,
                RecordID = recordId,
                PatientID = patientId,
                UploadedAt = DateTime.Now
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Attachment>.Ok(attachment, "تم رفع الملف بنجاح."));
        }

        // GET: api/attachments/record/{recordId}
        [HttpGet("record/{recordId}")]
        public async Task<IActionResult> GetRecordAttachments(int recordId)
        {
            var record = await _context.MedicalRecords.Include(m => m.Appointment).AsNoTracking().FirstOrDefaultAsync(m => m.RecordID == recordId);
            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي غير موجود."));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (record.Appointment.DoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                var patientIdOwn = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                if (record.Appointment.PatientID != patientIdOwn)
                    return Forbid();
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            var attachments = await _context.Attachments
                .Where(a => a.RecordID == recordId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<Attachment>>.Ok(attachments));
        }

        // GET: api/attachments/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientAttachments(int patientId)
        {
            var patient = await _context.PatientProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.PatientID == patientId);
            if (patient == null)
                return NotFound(ApiResponse.Fail("الملف الطبي للمريض غير موجود."));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Patient")
            {
                var patientIdOwn = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                if (patientId != patientIdOwn)
                    return Forbid();
            }
            else if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                var hasRelationship = await _context.Appointments.AnyAsync(a => a.PatientID == patientId && a.DoctorID == doctorId);
                if (!hasRelationship)
                    return Forbid();
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            var attachments = await _context.Attachments
                .Where(a => a.PatientID == patientId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<Attachment>>.Ok(attachments));
        }

        // DELETE: api/attachments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null)
                return NotFound(ApiResponse.Fail("الملف غير موجود."));

            var role = JwtHelper.GetUserRoleFromClaims(User);
            var userId = JwtHelper.GetUserIdFromClaims(User);
            // Only Doctors or Admins can delete
            if (role == "Patient")
                return Forbid();

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (attachment.RecordID.HasValue)
                {
                    var recordDoctorId = await _context.MedicalRecords
                        .Where(m => m.RecordID == attachment.RecordID.Value)
                        .Select(m => m.Appointment.DoctorID)
                        .FirstOrDefaultAsync();
                    if (recordDoctorId != doctorId)
                        return Forbid();
                }
                else if (attachment.PatientID.HasValue)
                {
                    var hasRelationship = await _context.Appointments.AnyAsync(a => a.PatientID == attachment.PatientID.Value && a.DoctorID == doctorId);
                    if (!hasRelationship)
                        return Forbid();
                }
                else
                {
                    return Forbid();
                }
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            // Delete physical file
            var filePath = Path.Combine(_env.WebRootPath, attachment.FileURL.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Remove from DB
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف الملف بنجاح."));
        }
    }
}
