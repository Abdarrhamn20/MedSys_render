using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Models;
using System.Security.Claims;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RadiologyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RadiologyController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private string GetUserRole()
        {
            var claim = User.FindFirst(ClaimTypes.Role);
            return claim != null ? claim.Value : "";
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _context.RadiologyTemplates.ToListAsync();

            if (templates.Count == 0)
            {
                // Seed default report templates
                var t1 = new RadiologyTemplate
                {
                    TemplateName = "أشعة سينية للصدر طبيعية (Chest X-Ray Normal)",
                    Modality = "X-Ray",
                    BodyPart = "Chest",
                    DefaultReportText = "PA & Lateral view of chest shows normal lung fields. Heart size is normal. Both costophrenic angles are clear. No evidence of active lung lesions."
                };

                var t2 = new RadiologyTemplate
                {
                    TemplateName = "موجات فوق صوتية للبطن طبيعية (Abdomen Ultrasound Normal)",
                    Modality = "Ultrasound",
                    BodyPart = "Abdomen",
                    DefaultReportText = "Ultrasonic examination of abdomen shows normal size, shape, and echogenicity of liver, gallbladder, spleen, pancreas, and both kidneys. No focal lesions or ascites."
                };

                _context.RadiologyTemplates.AddRange(t1, t2);
                await _context.SaveChangesAsync();

                templates = await _context.RadiologyTemplates.ToListAsync();
            }

            return Ok(ApiResponse<object>.Ok(templates, "قوالب تقارير الأشعة القياسية"));
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetRadiologyOrders([FromQuery] string? status, [FromQuery] int? patientUserId)
        {
            var currentUserId = GetUserId();
            var currentRole = GetUserRole();

            var query = _context.RadiologyOrders
                .Include(o => o.PatientUser)
                .Include(o => o.Doctor)
                .Include(o => o.Radiologist)
                .AsQueryable();

            if (currentRole == "Patient")
            {
                query = query.Where(o => o.PatientUserID == currentUserId);
            }
            else if (currentRole == "Doctor")
            {
                query = query.Where(o => o.DoctorID == currentUserId);
            }
            else if (patientUserId.HasValue)
            {
                query = query.Where(o => o.PatientUserID == patientUserId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query.OrderByDescending(o => o.RequestedAt).ToListAsync();
            return Ok(ApiResponse<object>.Ok(orders, "قائمة طلبات الأشعة التشخيصية"));
        }

        [HttpPost("orders")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreateRadiologyOrder([FromBody] CreateRadiologyOrderDTO dto)
        {
            // التحقق من وجود المريض قبل الإنشاء (يُجنّب أخطاء القيود المرجعية)
            if (!await _context.Users.AnyAsync(u => u.UserID == dto.PatientUserID))
                return BadRequest(ApiResponse.Fail("المريض غير موجود"));

            if (string.IsNullOrWhiteSpace(dto.Modality) || string.IsNullOrWhiteSpace(dto.BodyPart))
                return BadRequest(ApiResponse.Fail("نوع الفحص والمنطقة مطلوبان"));

            var doctorId = GetUserRole() == "Doctor" ? GetUserId() : (dto.DoctorID > 0 ? dto.DoctorID : GetUserId());

            var order = new RadiologyOrder
            {
                PatientUserID = dto.PatientUserID,
                DoctorID = doctorId,
                Modality = dto.Modality,
                BodyPart = dto.BodyPart,
                Status = "Requested",
                RequestedAt = DateTime.Now
            };

            _context.RadiologyOrders.Add(order);
            await _context.SaveChangesAsync();

            // إنشاء فاتورة أشعة تلقائية
            decimal templatePrice = 0;
            if (dto.TemplateID.HasValue)
            {
                templatePrice = await _context.RadiologyTemplates
                    .Where(t => t.TemplateID == dto.TemplateID.Value)
                    .Select(t => t.Price)
                    .FirstOrDefaultAsync();
            }
            else
            {
                templatePrice = await _context.RadiologyTemplates
                    .Where(t => t.Modality == dto.Modality)
                    .Select(t => t.Price)
                    .FirstOrDefaultAsync();
            }

            if (templatePrice > 0)
            {
                var invoice = new Invoice
                {
                    PatientUserID = dto.PatientUserID,
                    DoctorID = doctorId,
                    RadiologyOrderID = order.RadiologyOrderID,
                    InvoiceType = "Radiology",
                    Amount = templatePrice,
                    Tax = 0.00m,
                    Discount = 0.00m,
                    TotalAmount = templatePrice,
                    Status = "Unpaid",
                    CreatedAt = DateTime.Now
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse<object>.Ok(new { order, templatePrice }, "تم طلب فحوصات الأشعة وفاتورة المريض بنجاح"));
        }

        [HttpPut("orders/{id}/report")]
        [Authorize(Roles = "Admin,Doctor,Radiologist")]
        public async Task<IActionResult> UpdateRadiologyReport(int id, [FromBody] UpdateRadiologyReportDTO dto)
        {
            var order = await _context.RadiologyOrders.FirstOrDefaultAsync(o => o.RadiologyOrderID == id);
            if (order == null)
            {
                return NotFound(ApiResponse.Fail("طلب الأشعة غير موجود."));
            }

            // الطبيب يعتمد تقرير طلباته فقط (الأخصائي والأدمن يدخلون التقارير)
            if (GetUserRole() == "Doctor" && order.DoctorID != GetUserId())
                return Forbid();

            if (string.IsNullOrWhiteSpace(dto.ReportText))
                return BadRequest(ApiResponse.Fail("نص التقرير مطلوب"));

            order.ReportText = dto.ReportText;
            if (!string.IsNullOrEmpty(dto.ImagePath))
            {
                order.ImagePath = dto.ImagePath.Trim().Trim('"').Trim();
            }
            order.Status = "Completed";
            order.RadiologistID = GetUserId();
            order.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(order, "تم اعتماد تقرير الأشعة وإرفاقه بالملف الطبي الإلكتروني بنجاح"));
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Doctor,Radiologist")]
        [RequestSizeLimit(15L * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse.Fail("لم يتم اختيار ملف صورة."));

            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !allowedExt.Contains(ext))
                return BadRequest(ApiResponse.Fail("نوع الملف غير مدعوم. الصيغ المسموحة: JPG, PNG, GIF, BMP, WEBP"));

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest(ApiResponse.Fail("حجم الصورة يتجاوز الحد الأقصى (10MB)."));

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "radiology");
            Directory.CreateDirectory(uploadsDir);

            var safeName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, safeName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/radiology/{safeName}";
            return Ok(ApiResponse<object>.Ok(url, "تم رفع صورة الأشعة بنجاح."));
        }
    }
}
