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
    [Authorize(Roles = "Admin")]
    public class PriceManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PriceManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        // =============================================
        //  OVERVIEW (نظرة عامة على جميع الأسعار)
        // =============================================

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var doctorsCount = await _context.DoctorProfiles.CountAsync();
            var labTestsCount = await _context.LabTests.CountAsync();
            var radiologyTemplatesCount = await _context.RadiologyTemplates.CountAsync();
            var medicationsCount = await _context.Medications.CountAsync(m => m.IsActive);
            var roomsCount = await _context.Rooms.CountAsync(r => r.IsActive);
            var inventoryCount = await _context.InventoryItems.CountAsync(i => i.IsActive);
            var healthServicesCount = await _context.HealthServices.CountAsync(h => h.IsActive);

            return Ok(ApiResponse<object>.Ok(new
            {
                doctorsCount,
                labTestsCount,
                radiologyTemplatesCount,
                medicationsCount,
                roomsCount,
                inventoryCount,
                healthServicesCount
            }, "نظرة عامة على فئات الأسعار"));
        }

        // =============================================
        //  DOCTORS (رسوم الكشف)
        // =============================================

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctorFees()
        {
            var doctors = await _context.DoctorProfiles
                .Include(d => d.User)
                .Select(d => new
                {
                    d.DoctorID,
                    d.User.FullName,
                    d.User.Email,
                    d.Specialty,
                    d.ConsultationFee
                })
                .OrderBy(d => d.FullName)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(doctors, "رسوم الكشف"));
        }

        [HttpPut("doctors/{id}")]
        public async Task<IActionResult> UpdateDoctorFee(int id, [FromBody] UpdatePriceDTO dto)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor == null)
                return NotFound(ApiResponse.Fail("الطبيب غير موجود"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            doctor.ConsultationFee = dto.Price;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث رسوم الكشف إلى {dto.Price} د.ل"));
        }

        // =============================================
        //  LAB TESTS (أسعار التحاليل)
        // =============================================

        [HttpGet("lab-tests")]
        public async Task<IActionResult> GetLabTestPrices()
        {
            var tests = await _context.LabTests
                .Select(t => new
                {
                    t.LabTestID,
                    t.TestName,
                    t.Code,
                    t.Category,
                    t.Price,
                    t.Unit,
                    t.IsPanel
                })
                .OrderBy(t => t.Category).ThenBy(t => t.TestName)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(tests, "أسعار التحاليل"));
        }

        [HttpPut("lab-tests/{id}")]
        public async Task<IActionResult> UpdateLabTestPrice(int id, [FromBody] UpdatePriceDTO dto)
        {
            var test = await _context.LabTests.FindAsync(id);
            if (test == null)
                return NotFound(ApiResponse.Fail("الفحص غير موجود"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            test.Price = dto.Price;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث سعر فحص {test.TestName} إلى {dto.Price} د.ل"));
        }

        // =============================================
        //  RADIOLOGY TEMPLATES (أسعار الأشعة)
        // =============================================

        [HttpGet("radiology-templates")]
        public async Task<IActionResult> GetRadiologyPrices()
        {
            var templates = await _context.RadiologyTemplates
                .Select(t => new
                {
                    t.TemplateID,
                    t.TemplateName,
                    t.Modality,
                    t.BodyPart,
                    t.Price
                })
                .OrderBy(t => t.Modality).ThenBy(t => t.TemplateName)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(templates, "أسعار الأشعة"));
        }

        [HttpPut("radiology-templates/{id}")]
        public async Task<IActionResult> UpdateRadiologyPrice(int id, [FromBody] UpdatePriceDTO dto)
        {
            var template = await _context.RadiologyTemplates.FindAsync(id);
            if (template == null)
                return NotFound(ApiResponse.Fail("قالب الأشعة غير موجود"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            template.Price = dto.Price;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث سعر قالب {template.TemplateName} إلى {dto.Price} د.ل"));
        }

        // =============================================
        //  MEDICATIONS (أسعار الأدوية)
        // =============================================

        [HttpGet("medications")]
        public async Task<IActionResult> GetMedicationPrices([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var query = _context.Medications
                .Where(m => m.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Name.Contains(search) || m.NameAr.Contains(search));

            var totalCount = await query.CountAsync();

            var meds = await query
                .OrderBy(m => m.NameAr)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MedicationID,
                    m.Name,
                    m.NameAr,
                    m.Category,
                    m.PurchasePrice,
                    m.SellingPrice,
                    m.QuantityInStock,
                    m.Unit
                })
                .ToListAsync();

            return Ok(new { success = true, data = meds, totalCount, page, pageSize });
        }

        [HttpPut("medications/{id}/prices")]
        public async Task<IActionResult> UpdateMedicationPrices(int id, [FromBody] UpdateMedicationPricesDTO dto)
        {
            var med = await _context.Medications.FindAsync(id);
            if (med == null)
                return NotFound(ApiResponse.Fail("الدواء غير موجود"));

            if (dto.PurchasePrice < 0 || dto.SellingPrice < 0)
                return BadRequest(ApiResponse.Fail("الأسعار لا يمكن أن تكون سالبة"));

            med.PurchasePrice = dto.PurchasePrice;
            med.SellingPrice = dto.SellingPrice;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث أسعار دواء {med.NameAr}: شراء {dto.PurchasePrice} / بيع {dto.SellingPrice} د.ل"));
        }

        // =============================================
        //  ROOMS (أسعار الغرف اليومية)
        // =============================================

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRoomPrices()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Ward)
                .Where(r => r.IsActive)
                .Select(r => new
                {
                    r.RoomID,
                    r.RoomNumber,
                    r.RoomType,
                    r.DailyRate,
                    r.MaxBeds,
                    WardName = r.Ward.WardName
                })
                .OrderBy(r => r.WardName).ThenBy(r => r.RoomNumber)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(rooms, "أسعار الغرف اليومية"));
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoomRate(int id, [FromBody] UpdatePriceDTO dto)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound(ApiResponse.Fail("الغرفة غير موجودة"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            room.DailyRate = dto.Price;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث سعر الغرفة {room.RoomNumber} إلى {dto.Price} د.ل/يوم"));
        }

        // =============================================
        //  INVENTORY (أسعار المخزون)
        // =============================================

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryPrices([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var query = _context.InventoryItems
                .Include(i => i.Category)
                .Where(i => i.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.ItemName.Contains(search) || i.ItemNameAr.Contains(search));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(i => i.ItemNameAr)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new
                {
                    i.ItemID,
                    i.ItemCode,
                    i.ItemName,
                    i.ItemNameAr,
                    CategoryName = i.Category.CategoryName,
                    i.PurchasePrice,
                    i.SellingPrice,
                    i.Unit,
                    i.ReorderLevel
                })
                .ToListAsync();

            return Ok(new { success = true, data = items, totalCount, page, pageSize });
        }

        [HttpPut("inventory/{id}/prices")]
        public async Task<IActionResult> UpdateInventoryPrices(int id, [FromBody] UpdateInventoryPricesDTO dto)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound(ApiResponse.Fail("الصنف غير موجود"));

            if (dto.PurchasePrice < 0 || dto.SellingPrice < 0)
                return BadRequest(ApiResponse.Fail("الأسعار لا يمكن أن تكون سالبة"));

            item.PurchasePrice = dto.PurchasePrice;
            item.SellingPrice = dto.SellingPrice;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث أسعار صنف {item.ItemNameAr}: شراء {dto.PurchasePrice} / بيع {dto.SellingPrice} د.ل"));
        }

        // =============================================
        //  BULK UPDATE (تحديث جماعي للأسعار)
        // =============================================

        [HttpPost("bulk-update")]
        public async Task<IActionResult> BulkUpdatePrices([FromBody] BulkPriceUpdateDTO dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(ApiResponse.Fail("لا توجد عناصر للتحديث"));

            int updated = 0;

            foreach (var item in dto.Items)
            {
                if (item.Price < 0) continue;

                switch (item.EntityType?.ToLower())
                {
                    case "doctor":
                        var doctor = await _context.DoctorProfiles.FindAsync(item.EntityID);
                        if (doctor != null) { doctor.ConsultationFee = item.Price; updated++; }
                        break;
                    case "labtest":
                        var test = await _context.LabTests.FindAsync(item.EntityID);
                        if (test != null) { test.Price = item.Price; updated++; }
                        break;
                    case "radiology":
                        var tpl = await _context.RadiologyTemplates.FindAsync(item.EntityID);
                        if (tpl != null) { tpl.Price = item.Price; updated++; }
                        break;
                    case "room":
                        var room = await _context.Rooms.FindAsync(item.EntityID);
                        if (room != null) { room.DailyRate = item.Price; updated++; }
                        break;
                }
            }

            if (updated > 0)
                await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث {updated} من {dto.Items.Count} عنصر بنجاح"));
        }

        // =============================================
        //  PRICE INCREASE/DECREASE (نسبة مئوية)
        // =============================================

        [HttpPost("adjust-prices")]
        public async Task<IActionResult> AdjustPrices([FromBody] AdjustPricesDTO dto)
        {
            if (dto.Percentage == 0)
                return BadRequest(ApiResponse.Fail("النسبة لا يمكن أن تكون صفر"));

            int affected = 0;

            switch (dto.EntityType?.ToLower())
            {
                case "doctors":
                    var doctors = await _context.DoctorProfiles.ToListAsync();
                    foreach (var d in doctors)
                    {
                        d.ConsultationFee = Math.Round(d.ConsultationFee * (1 + dto.Percentage / 100m), 2);
                    }
                    affected = doctors.Count;
                    break;

                case "labtests":
                    var tests = await _context.LabTests.ToListAsync();
                    foreach (var t in tests)
                    {
                        t.Price = Math.Round(t.Price * (1 + dto.Percentage / 100m), 2);
                    }
                    affected = tests.Count;
                    break;

                case "radiology":
                    var templates = await _context.RadiologyTemplates.ToListAsync();
                    foreach (var t in templates)
                    {
                        t.Price = Math.Round(t.Price * (1 + dto.Percentage / 100m), 2);
                    }
                    affected = templates.Count;
                    break;

                case "rooms":
                    var rooms = await _context.Rooms.Where(r => r.IsActive).ToListAsync();
                    foreach (var r in rooms)
                    {
                        r.DailyRate = Math.Round(r.DailyRate * (1 + dto.Percentage / 100m), 2);
                    }
                    affected = rooms.Count;
                    break;

                case "medications":
                    var meds = await _context.Medications.Where(m => m.IsActive).ToListAsync();
                    foreach (var m in meds)
                    {
                        m.SellingPrice = Math.Round(m.SellingPrice * (1 + dto.Percentage / 100m), 2);
                    }
                    affected = meds.Count;
                    break;

                case "inventory":
                    var items = await _context.InventoryItems.Where(i => i.IsActive).ToListAsync();
                    foreach (var i in items)
                    {
                        i.SellingPrice = Math.Round(i.SellingPrice * (1 + dto.Percentage / 100m), 2);
                    }
                    affected = items.Count;
                    break;

                default:
                    return BadRequest(ApiResponse.Fail("نوع الكيان غير صالح. الأنواع المدعومة: doctors, labtests, radiology, rooms, medications, inventory"));
            }

            if (affected > 0)
                await _context.SaveChangesAsync();

            var sign = dto.Percentage > 0 ? "+" : "";
            return Ok(ApiResponse.Ok($"تم تعديل أسعار {affected} عنصر بنسبة {sign}{dto.Percentage}% بنجاح"));
        }

        // =============================================
        //  CREATE LAB TEST (إضافة فحص تحاليل جديد)
        // =============================================

        [HttpPost("lab-tests")]
        public async Task<IActionResult> CreateLabTest([FromBody] CreateLabTestPriceDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TestName) || string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest(ApiResponse.Fail("اسم الفحص وكوده مطلوبان"));

            if (await _context.LabTests.AnyAsync(t => t.Code == dto.Code.Trim().ToUpperInvariant()))
                return BadRequest(ApiResponse.Fail("كود الفحص مسجل مسبقاً"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            var test = new LabTest
            {
                TestName = dto.TestName.Trim(),
                Code = dto.Code.Trim().ToUpperInvariant(),
                Category = dto.Category ?? "General",
                Price = dto.Price,
                Unit = dto.Unit ?? "mg/dL",
                IsPanel = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.LabTests.Add(test);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(test, $"تم إنشاء فحص {test.TestName} بنجاح"));
        }

        // =============================================
        //  CREATE RADIOLOGY TEMPLATE (إضافة قالب أشعة جديد)
        // =============================================

        [HttpPost("radiology-templates")]
        public async Task<IActionResult> CreateRadiologyTemplate([FromBody] CreateRadiologyTemplatePriceDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TemplateName) || string.IsNullOrWhiteSpace(dto.Modality))
                return BadRequest(ApiResponse.Fail("اسم القالب والطريقة مطلوبان"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            var template = new RadiologyTemplate
            {
                TemplateName = dto.TemplateName.Trim(),
                Modality = dto.Modality.Trim(),
                BodyPart = dto.BodyPart ?? "General",
                DefaultReportText = dto.DefaultReportText ?? "",
                Price = dto.Price,
                CreatedAt = DateTime.UtcNow
            };

            _context.RadiologyTemplates.Add(template);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(template, $"تم إنشاء قالب {template.TemplateName} بنجاح"));
        }

        // =============================================
        //  HEALTH SERVICES (الخدمات الصحية)
        // =============================================

        [HttpGet("health-services")]
        public async Task<IActionResult> GetHealthServices()
        {
            var services = await _context.HealthServices
                .Where(h => h.IsActive)
                .OrderBy(h => h.Category).ThenBy(h => h.ServiceNameAr)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(services, "الخدمات الصحية"));
        }

        [HttpPost("health-services")]
        public async Task<IActionResult> CreateHealthService([FromBody] CreateHealthServiceDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ServiceName) || string.IsNullOrWhiteSpace(dto.ServiceNameAr))
                return BadRequest(ApiResponse.Fail("اسم الخدمة بالعربية والإنجليزية مطلوب"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            var service = new HealthService
            {
                ServiceName = dto.ServiceName.Trim(),
                ServiceNameAr = dto.ServiceNameAr.Trim(),
                Category = dto.Category ?? "General",
                Description = dto.Description,
                Price = dto.Price,
                Unit = dto.Unit ?? "مرة",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.HealthServices.Add(service);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(service, $"تم إنشاء الخدمة {service.ServiceNameAr} بنجاح"));
        }

        [HttpPut("health-services/{id}")]
        public async Task<IActionResult> UpdateHealthService(int id, [FromBody] UpdateHealthServiceDTO dto)
        {
            var service = await _context.HealthServices.FindAsync(id);
            if (service == null)
                return NotFound(ApiResponse.Fail("الخدمة غير موجودة"));

            if (dto.Price < 0)
                return BadRequest(ApiResponse.Fail("السعر لا يمكن أن يكون سالباً"));

            if (!string.IsNullOrWhiteSpace(dto.ServiceName)) service.ServiceName = dto.ServiceName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.ServiceNameAr)) service.ServiceNameAr = dto.ServiceNameAr.Trim();
            if (dto.Category != null) service.Category = dto.Category;
            if (dto.Description != null) service.Description = dto.Description;
            service.Price = dto.Price;
            if (dto.Unit != null) service.Unit = dto.Unit;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديث الخدمة {service.ServiceNameAr} بنجاح"));
        }

        [HttpDelete("health-services/{id}")]
        public async Task<IActionResult> DeleteHealthService(int id)
        {
            var service = await _context.HealthServices.FindAsync(id);
            if (service == null)
                return NotFound(ApiResponse.Fail("الخدمة غير موجودة"));

            service.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم حذف الخدمة {service.ServiceNameAr} بنجاح"));
        }

        // =============================================
        //  DELETE LAB TEST / RADIOLOGY TEMPLATE
        // =============================================

        [HttpDelete("lab-tests/{id}")]
        public async Task<IActionResult> DeleteLabTestPrice(int id)
        {
            var test = await _context.LabTests.FindAsync(id);
            if (test == null)
                return NotFound(ApiResponse.Fail("الفحص غير موجود"));

            var inUse = await _context.LabOrderItems.AnyAsync(i => i.LabTestID == id);
            if (inUse)
                return BadRequest(ApiResponse.Fail("لا يمكن حذف الفحص لارتباطه بطلبات سابقة"));

            _context.LabTests.Remove(test);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم حذف فحص {test.TestName} بنجاح"));
        }

        [HttpDelete("radiology-templates/{id}")]
        public async Task<IActionResult> DeleteRadiologyTemplatePrice(int id)
        {
            var template = await _context.RadiologyTemplates.FindAsync(id);
            if (template == null)
                return NotFound(ApiResponse.Fail("القالب غير موجود"));

            _context.RadiologyTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم حذف قالب {template.TemplateName} بنجاح"));
        }
    }

    // =============================================
    //  DTOs for Price Management
    // =============================================

    public class UpdatePriceDTO
    {
        public decimal Price { get; set; }
    }

    public class UpdateMedicationPricesDTO
    {
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
    }

    public class UpdateInventoryPricesDTO
    {
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
    }

    public class BulkPriceUpdateDTO
    {
        public List<BulkPriceItem> Items { get; set; } = new();
    }

    public class BulkPriceItem
    {
        public string EntityType { get; set; } = string.Empty;
        public int EntityID { get; set; }
        public decimal Price { get; set; }
    }

    public class AdjustPricesDTO
    {
        public string EntityType { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
    }

    public class CreateLabTestPriceDTO
    {
        public string TestName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
    }

    public class CreateRadiologyTemplatePriceDTO
    {
        public string TemplateName { get; set; } = string.Empty;
        public string Modality { get; set; } = string.Empty;
        public string? BodyPart { get; set; }
        public string? DefaultReportText { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateHealthServiceDTO
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceNameAr { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
    }

    public class UpdateHealthServiceDTO
    {
        public string? ServiceName { get; set; }
        public string? ServiceNameAr { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
    }
}
