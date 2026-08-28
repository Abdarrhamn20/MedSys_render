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
    public class LabController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LabController(ApplicationDbContext context)
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

        // ============================================================
        //  الفحوصات والبانلات
        // ============================================================

        [HttpGet("tests")]
        public async Task<IActionResult> GetLabTests()
        {
            var tests = await _context.LabTests
                .Include(t => t.ReferenceRanges)
                .Include(t => t.PanelChildren)
                .Include(t => t.Device)
                .ToListAsync();

            if (tests.Count == 0)
            {
                // Seed default lab tests if empty
                var cbc = new LabTest { TestName = "صورة دم كاملة (CBC)", Code = "CBC", Category = "Hematology", Price = 30.00m, Unit = "g/dL" };
                cbc.ReferenceRanges.Add(new LabReferenceRange { Gender = "All", MinAge = 0, MaxAge = 120, NormalMin = 11.5m, NormalMax = 16.5m });

                var fbs = new LabTest { TestName = "السكر الصائم (FBS)", Code = "FBS", Category = "Biochemistry", Price = 20.00m, Unit = "mg/dL" };
                fbs.ReferenceRanges.Add(new LabReferenceRange { Gender = "All", MinAge = 0, MaxAge = 120, NormalMin = 70.0m, NormalMax = 99.0m });

                var hba1c = new LabTest { TestName = "السكر التراكمي (HbA1c)", Code = "HBA1C", Category = "Biochemistry", Price = 45.00m, Unit = "%" };
                hba1c.ReferenceRanges.Add(new LabReferenceRange { Gender = "All", MinAge = 0, MaxAge = 120, NormalMin = 4.0m, NormalMax = 5.6m });

                _context.LabTests.AddRange(cbc, fbs, hba1c);
                await _context.SaveChangesAsync();

                tests = await _context.LabTests
                    .Include(t => t.ReferenceRanges)
                    .Include(t => t.PanelChildren)
                    .Include(t => t.Device)
                    .ToListAsync();
            }

            return Ok(ApiResponse<object>.Ok(tests, "قائمة الفحوصات والتحاليل الطبية"));
        }

        [HttpPost("tests")]
        [Authorize(Roles = "Admin,LabTechnician")]
        public async Task<IActionResult> CreateLabTest([FromBody] LabTestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TestName) || string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest(ApiResponse.Fail("اسم الفحص وكوده مطلوبان."));

            if (await _context.LabTests.AnyAsync(t => t.Code == dto.Code))
                return BadRequest(ApiResponse.Fail("كود الفحص مسجل مسبقاً."));

            var test = new LabTest
            {
                TestName = dto.TestName.Trim(),
                Code = dto.Code.Trim().ToUpperInvariant(),
                Category = dto.Category ?? "General",
                Price = dto.Price,
                Unit = dto.Unit ?? "mg/dL",
                IsPanel = dto.IsPanel,
                PanelID = dto.PanelID,
                DeviceID = dto.DeviceID,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var r in dto.ReferenceRanges)
            {
                test.ReferenceRanges.Add(new LabReferenceRange
                {
                    Gender = string.IsNullOrWhiteSpace(r.Gender) ? "All" : r.Gender,
                    MinAge = r.MinAge,
                    MaxAge = r.MaxAge,
                    NormalMin = r.NormalMin,
                    NormalMax = r.NormalMax,
                    RangeNotes = r.RangeNotes
                });
            }

            _context.LabTests.Add(test);
            await _context.SaveChangesAsync();

            await AuditAsync("LabTestCreated", "LabTest", test.LabTestID, $"إنشاء فحص {test.TestName} ({test.Code})");
            return Ok(ApiResponse<object>.Ok(test, "تم إنشاء الفحص بنجاح"));
        }

        [HttpPut("tests/{id}")]
        [Authorize(Roles = "Admin,LabTechnician")]
        public async Task<IActionResult> UpdateLabTest(int id, [FromBody] LabTestDTO dto)
        {
            var test = await _context.LabTests
                .Include(t => t.ReferenceRanges)
                .FirstOrDefaultAsync(t => t.LabTestID == id);
            if (test == null)
                return NotFound(ApiResponse.Fail("الفحص غير موجود."));

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                if (await _context.LabTests.AnyAsync(t => t.Code == dto.Code && t.LabTestID != id))
                    return BadRequest(ApiResponse.Fail("كود الفحص مسجل مسبقاً."));
                test.Code = dto.Code.Trim().ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(dto.TestName)) test.TestName = dto.TestName.Trim();
            if (dto.Category != null) test.Category = dto.Category;
            test.Price = dto.Price;
            if (dto.Unit != null) test.Unit = dto.Unit;
            test.IsPanel = dto.IsPanel;
            test.PanelID = dto.PanelID;
            test.DeviceID = dto.DeviceID;

            // تحديث المعايير المرجعية: حذف الموجود وإعادة الإضافة
            _context.LabReferenceRanges.RemoveRange(test.ReferenceRanges);
            foreach (var r in dto.ReferenceRanges)
            {
                test.ReferenceRanges.Add(new LabReferenceRange
                {
                    Gender = string.IsNullOrWhiteSpace(r.Gender) ? "All" : r.Gender,
                    MinAge = r.MinAge,
                    MaxAge = r.MaxAge,
                    NormalMin = r.NormalMin,
                    NormalMax = r.NormalMax,
                    RangeNotes = r.RangeNotes
                });
            }

            await _context.SaveChangesAsync();
            await AuditAsync("LabTestUpdated", "LabTest", test.LabTestID, $"تعديل فحص {test.TestName} ({test.Code})");
            return Ok(ApiResponse.Ok("تم تحديث الفحص بنجاح"));
        }

        [HttpDelete("tests/{id}")]
        [Authorize(Roles = "Admin,LabTechnician")]
        public async Task<IActionResult> DeleteLabTest(int id)
        {
            var test = await _context.LabTests.FirstOrDefaultAsync(t => t.LabTestID == id);
            if (test == null)
                return NotFound(ApiResponse.Fail("الفحص غير موجود."));

            // منع حذف فحص مرجعي من قبل عناصر طلبات قائمة أو بانل أب
            var inUse = await _context.LabOrderItems.AnyAsync(i => i.LabTestID == id);
            if (inUse)
                return BadRequest(ApiResponse.Fail("لا يمكن حذف الفحص لارتباطه بطلبات سابقة."));

            _context.LabTests.Remove(test);
            await _context.SaveChangesAsync();
            await AuditAsync("LabTestDeleted", "LabTest", id, $"حذف فحص {test.TestName}");
            return Ok(ApiResponse.Ok("تم حذف الفحص بنجاح"));
        }

        [HttpPost("tests/{id}/panel")]
        [Authorize(Roles = "Admin,LabTechnician")]
        public async Task<IActionResult> AddPanelMember(int id, [FromBody] AddPanelMemberDTO dto)
        {
            var panel = await _context.LabTests.FirstOrDefaultAsync(t => t.LabTestID == id);
            if (panel == null)
                return NotFound(ApiResponse.Fail("البانل غير موجود."));
            if (!panel.IsPanel)
                return BadRequest(ApiResponse.Fail("الفحص المحدد ليس بانلاً مركباً."));

            var member = await _context.LabTests.FirstOrDefaultAsync(t => t.LabTestID == dto.MemberTestID);
            if (member == null)
                return NotFound(ApiResponse.Fail("الفحص الفرعي غير موجود."));
            if (member.IsPanel)
                return BadRequest(ApiResponse.Fail("لا يمكن إضافة بانل داخل بانل."));
            if (member.PanelID == panel.LabTestID)
                return BadRequest(ApiResponse.Fail("الفحص مضاف مسبقاً إلى البانل."));

            member.PanelID = panel.LabTestID;
            await _context.SaveChangesAsync();
            await AuditAsync("LabPanelMemberAdded", "LabTest", id, $"إضافة {member.TestName} إلى البانل {panel.TestName}");
            return Ok(ApiResponse.Ok($"تمت إضافة {member.TestName} إلى البانل {panel.TestName} بنجاح"));
        }

        // ============================================================
        //  الطلبات والعناصر
        // ============================================================

        [HttpGet("orders")]
        public async Task<IActionResult> GetLabOrders([FromQuery] string? status, [FromQuery] int? patientUserId)
        {
            var currentUserId = GetUserId();
            var currentRole = GetUserRole();

            var query = _context.LabOrders
                .Include(o => o.PatientUser)
                .Include(o => o.Doctor)
                .Include(o => o.LabTest)
                    .ThenInclude(t => t!.ReferenceRanges)
                .Include(o => o.Items)
                    .ThenInclude(i => i!.LabTest)
                        .ThenInclude(t => t!.ReferenceRanges)
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
            return Ok(ApiResponse<object>.Ok(orders, "قائمة طلبات التحاليل الطبية"));
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetLabOrder(int id)
        {
            var currentUserId = GetUserId();
            var currentRole = GetUserRole();

            var order = await _context.LabOrders
                .Include(o => o.PatientUser)
                .Include(o => o.Doctor)
                .Include(o => o.LabTest)
                    .ThenInclude(t => t!.ReferenceRanges)
                .Include(o => o.Items)
                    .ThenInclude(i => i!.LabTest)
                        .ThenInclude(t => t!.ReferenceRanges)
                .Include(o => o.Items)
                    .ThenInclude(i => i!.LabTest)
                        .ThenInclude(t => t!.PanelChildren)
                .FirstOrDefaultAsync(o => o.LabOrderID == id);

            if (order == null)
                return NotFound(ApiResponse.Fail("طلب التحليل غير موجود."));

            if (currentRole == "Patient" && order.PatientUserID != currentUserId)
                return Forbid();
            if (currentRole == "Doctor" && order.DoctorID != currentUserId)
                return Forbid();

            return Ok(ApiResponse<object>.Ok(order));
        }

        [HttpPost("orders")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreateLabOrder([FromBody] CreateLabOrderDTO dto)
        {
            // التحقق من وجود المريض
            if (!await _context.Users.AnyAsync(u => u.UserID == dto.PatientUserID))
                return BadRequest(ApiResponse.Fail("المريض غير موجود"));

            var doctorId = GetUserRole() == "Doctor" ? GetUserId() : (dto.DoctorID > 0 ? dto.DoctorID : GetUserId());

            // جمع معرّفات الفحوصات: القائمة الجديدة أو الفحص الواحد (توافق خلفي)
            var testIds = new List<int>();
            if (dto.LabTestIDs != null && dto.LabTestIDs.Count > 0)
                testIds.AddRange(dto.LabTestIDs.Distinct());
            else if (dto.LabTestID > 0)
                testIds.Add(dto.LabTestID);

            if (testIds.Count == 0)
                return BadRequest(ApiResponse.Fail("يجب اختيار فحص واحد على الأقل."));

            // فكّ البانلات إلى فحوصاتها الفرعية
            var expanded = new List<int>();
            var requested = await _context.LabTests.Where(t => testIds.Contains(t.LabTestID)).ToListAsync();
            foreach (var t in requested)
            {
                if (t.IsPanel)
                {
                    var members = await _context.LabTests
                        .Where(x => x.PanelID == t.LabTestID)
                        .Select(x => x.LabTestID)
                        .ToListAsync();
                    if (members.Count == 0)
                        return BadRequest(ApiResponse.Fail($"البانل {t.TestName} لا يحتوي على فحوصات فرعية."));
                    expanded.AddRange(members);
                }
                else
                {
                    expanded.Add(t.LabTestID);
                }
            }
            expanded = expanded.Distinct().ToList();

            var order = new LabOrder
            {
                PatientUserID = dto.PatientUserID,
                DoctorID = doctorId,
                LabTestID = expanded[0],
                Status = "Requested",
                ResultStatus = "Pending",
                ResultNotes = dto.ResultNotes,
                RequestedAt = DateTime.Now
            };

            foreach (var tid in expanded)
            {
                order.Items.Add(new LabOrderItem
                {
                    LabTestID = tid,
                    ResultStatus = "Pending"
                });
            }

            _context.LabOrders.Add(order);
            await _context.SaveChangesAsync();

            // إنشاء فاتورة تحليلات تلقائية
            var testPrices = await _context.LabTests
                .Where(t => expanded.Contains(t.LabTestID))
                .Select(t => t.Price)
                .ToListAsync();
            var totalAmount = testPrices.Sum();

            if (totalAmount > 0)
            {
                var invoice = new Invoice
                {
                    PatientUserID = dto.PatientUserID,
                    DoctorID = doctorId,
                    LabOrderID = order.LabOrderID,
                    InvoiceType = "Laboratory",
                    Amount = totalAmount,
                    Tax = 0.00m,
                    Discount = 0.00m,
                    TotalAmount = totalAmount,
                    Status = "Unpaid",
                    CreatedAt = DateTime.Now
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
            }

            await AuditAsync("LabOrderCreated", "LabOrder", order.LabOrderID, $"إنشاء طلب تحليل للمريض ({dto.PatientUserID}) بعدد {order.Items.Count} فحص — إجمالي الفاتورة: {totalAmount} د.ل");
            return Ok(ApiResponse<object>.Ok(new { order, totalAmount }, "تم إنشاء طلب التحليل وفاتورة المريض بنجاح"));
        }

        [HttpPut("orders/{id}/items/{itemId}/result")]
        [Authorize(Roles = "Admin,Doctor,LabTechnician")]
        public async Task<IActionResult> UpdateLabResult(int id, int itemId, [FromBody] UpdateLabResultDTO dto)
        {
            var order = await _context.LabOrders
                .Include(o => o.PatientUser)
                .Include(o => o.Items)
                    .ThenInclude(i => i!.LabTest)
                        .ThenInclude(t => t!.ReferenceRanges)
                .FirstOrDefaultAsync(o => o.LabOrderID == id);

            if (order == null)
                return NotFound(ApiResponse.Fail("طلب التحليل غير موجود."));

            // الطبيب يعدّل نتيجة طلباته فقط
            if (GetUserRole() == "Doctor" && order.DoctorID != GetUserId())
                return Forbid();

            var item = order.Items.FirstOrDefault(i => i.LabOrderItemID == itemId);
            if (item == null)
                return NotFound(ApiResponse.Fail("عنصر التحليل غير موجود في هذا الطلب."));

            item.ResultValue = dto.ResultValue;
            item.TechnicianNotes = dto.TechnicianNotes;
            item.CompletedAt = DateTime.Now;
            item.ResultStatus = ComputeResultStatus(item, order.PatientUser);

            // بعد اكتمال كل العناصر → يكتمل الطلب
            if (order.Items.All(i => i.ResultStatus != "Pending"))
            {
                order.Status = "Completed";
                order.CompletedAt = DateTime.Now;
                order.VerificationQRCode = $"CLINICPRO-LAB-{order.LabOrderID}-{order.PatientUserID}-{DateTime.UtcNow.Ticks}";
            }
            else if (order.Status == "Requested")
            {
                order.Status = "InProgress";
            }

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(item, "تم إدخال نتيجة التحليل واحتساب المعدل الطبيعي آلياً بنجاح"));
        }

        // ============================================================
        //  المزرعة والحساسية
        // ============================================================

        [HttpPost("orders/{id}/items/{itemId}/culture")]
        [Authorize(Roles = "Admin,Doctor,LabTechnician")]
        public async Task<IActionResult> SaveCulture(int id, int itemId, [FromBody] CultureSensitivityDTO dto)
        {
            var item = await _context.LabOrderItems
                .FirstOrDefaultAsync(i => i.LabOrderItemID == itemId && i.LabOrderID == id);
            if (item == null)
                return NotFound(ApiResponse.Fail("عنصر التحليل غير موجود."));

            var existing = await _context.CultureSensitivities
                .Include(c => c.SensitivityResults)
                .FirstOrDefaultAsync(c => c.LabOrderItemID == itemId);

            if (existing != null)
            {
                existing.Organism = dto.Organism ?? existing.Organism;
                existing.GramStain = dto.GramStain ?? existing.GramStain;
                existing.QuantitativeResult = dto.QuantitativeResult ?? existing.QuantitativeResult;
                if (!string.IsNullOrWhiteSpace(dto.CultureStatus))
                    existing.CultureStatus = dto.CultureStatus;
                await _context.SaveChangesAsync();
                await AuditAsync("CultureUpdated", "CultureSensitivity", existing.CultureSensitivityID, $"تحديث مزرعة عنصر {itemId}");
                return Ok(ApiResponse<object>.Ok(existing, "تم تحديث بيانات المزرعة بنجاح"));
            }

            var culture = new CultureSensitivity
            {
                LabOrderItemID = itemId,
                Organism = dto.Organism,
                GramStain = dto.GramStain,
                CultureStatus = string.IsNullOrWhiteSpace(dto.CultureStatus) ? "NoGrowth" : dto.CultureStatus,
                QuantitativeResult = dto.QuantitativeResult,
                CreatedAt = DateTime.UtcNow
            };
            _context.CultureSensitivities.Add(culture);
            await _context.SaveChangesAsync();
            await AuditAsync("CultureCreated", "CultureSensitivity", culture.CultureSensitivityID, $"إضافة مزرعة لعنصر {itemId}");
            return Ok(ApiResponse<object>.Ok(culture, "تم حفظ بيانات المزرعة بنجاح"));
        }

        [HttpPost("culture/{cultureId}/sensitivities")]
        [Authorize(Roles = "Admin,Doctor,LabTechnician")]
        public async Task<IActionResult> AddSensitivity(int cultureId, [FromBody] SensitivityResultDTO dto)
        {
            var culture = await _context.CultureSensitivities
                .Include(c => c.SensitivityResults)
                .FirstOrDefaultAsync(c => c.CultureSensitivityID == cultureId);
            if (culture == null)
                return NotFound(ApiResponse.Fail("سجل المزرعة غير موجود."));

            var result = new SensitivityResult
            {
                CultureSensitivityID = cultureId,
                AntibioticName = dto.AntibioticName.Trim(),
                Interpretation = string.IsNullOrWhiteSpace(dto.Interpretation) ? "Sensitive" : dto.Interpretation,
                ZoneDiameter = dto.ZoneDiameter
            };
            _context.SensitivityResults.Add(result);
            await _context.SaveChangesAsync();
            await AuditAsync("SensitivityAdded", "SensitivityResult", result.SensitivityResultID, $"إضافة مضاد {result.AntibioticName} ({result.Interpretation}) للمزرعة {cultureId}");
            return Ok(ApiResponse<object>.Ok(result, "تمت إضافة نتيجة المضاد بنجاح"));
        }

        [HttpGet("orders/{id}/items/{itemId}/culture")]
        public async Task<IActionResult> GetCulture(int id, int itemId)
        {
            var culture = await _context.CultureSensitivities
                .Include(c => c.SensitivityResults)
                .FirstOrDefaultAsync(c => c.LabOrderItemID == itemId);
            if (culture == null)
                return NotFound(ApiResponse.Fail("لا توجد بيانات مزرعة لهذا العنصر بعد."));
            return Ok(ApiResponse<object>.Ok(culture));
        }

        // ============================================================
        //  الأجهزة
        // ============================================================

        [HttpGet("devices")]
        [Authorize(Roles = "Admin,LabTechnician")]
        public async Task<IActionResult> GetDevices()
        {
            var devices = await _context.LabDevices.OrderBy(d => d.DeviceName).ToListAsync();
            return Ok(ApiResponse<object>.Ok(devices));
        }

        [HttpPost("devices")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDevice([FromBody] LabDeviceDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DeviceName) || string.IsNullOrWhiteSpace(dto.DeviceCode))
                return BadRequest(ApiResponse.Fail("اسم الجهاز وكوده مطلوبان."));
            if (await _context.LabDevices.AnyAsync(d => d.DeviceCode == dto.DeviceCode))
                return BadRequest(ApiResponse.Fail("كود الجهاز مسجل مسبقاً."));

            var device = new LabDevice
            {
                DeviceName = dto.DeviceName.Trim(),
                DeviceCode = dto.DeviceCode.Trim().ToUpperInvariant(),
                DeviceModel = dto.DeviceModel,
                ConnectionType = string.IsNullOrWhiteSpace(dto.ConnectionType) ? "Manual" : dto.ConnectionType,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            _context.LabDevices.Add(device);
            await _context.SaveChangesAsync();
            await AuditAsync("LabDeviceCreated", "LabDevice", device.LabDeviceID, $"إضافة جهاز {device.DeviceName} ({device.DeviceCode})");
            return Ok(ApiResponse<object>.Ok(device, "تم إضافة الجهاز بنجاح"));
        }

        [HttpPut("devices/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] LabDeviceDTO dto)
        {
            var device = await _context.LabDevices.FirstOrDefaultAsync(d => d.LabDeviceID == id);
            if (device == null)
                return NotFound(ApiResponse.Fail("الجهاز غير موجود."));

            if (!string.IsNullOrWhiteSpace(dto.DeviceName)) device.DeviceName = dto.DeviceName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.DeviceCode))
            {
                if (await _context.LabDevices.AnyAsync(d => d.DeviceCode == dto.DeviceCode && d.LabDeviceID != id))
                    return BadRequest(ApiResponse.Fail("كود الجهاز مسجل مسبقاً."));
                device.DeviceCode = dto.DeviceCode.Trim().ToUpperInvariant();
            }
            if (dto.DeviceModel != null) device.DeviceModel = dto.DeviceModel;
            if (dto.ConnectionType != null) device.ConnectionType = dto.ConnectionType;
            device.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            await AuditAsync("LabDeviceUpdated", "LabDevice", device.LabDeviceID, $"تعديل جهاز {device.DeviceName}");
            return Ok(ApiResponse.Ok("تم تحديث الجهاز بنجاح"));
        }

        [HttpPost("devices/{id}/capture")]
        [Authorize(Roles = "Admin,LabTechnician")]
        public async Task<IActionResult> CaptureDeviceResult(int id, [FromBody] DeviceCaptureDTO dto)
        {
            var device = await _context.LabDevices.FirstOrDefaultAsync(d => d.LabDeviceID == id);
            if (device == null)
                return NotFound(ApiResponse.Fail("الجهاز غير موجود."));
            if (!device.IsActive)
                return BadRequest(ApiResponse.Fail("الجهاز غير مفعّل."));

            var item = await _context.LabOrderItems
                .Include(i => i.LabOrder)
                .Include(i => i.LabTest)
                    .ThenInclude(t => t!.ReferenceRanges)
                .FirstOrDefaultAsync(i => i.LabOrderItemID == dto.LabOrderItemID);
            if (item == null)
                return NotFound(ApiResponse.Fail("عنصر التحليل غير موجود."));

            // التأكد أن الفحص مرتبط بالجهاز (أو قبول التسجيل الإداري)
            if (item.LabTest != null && item.LabTest.DeviceID.HasValue && item.LabTest.DeviceID.Value != id)
                return BadRequest(ApiResponse.Fail("هذا الفحص لا يُنفذ على هذا الجهاز."));

            item.ResultValue = dto.Value;
            item.TechnicianNotes = string.IsNullOrWhiteSpace(dto.Notes)
                ? $"استُلم من الجهاز {device.DeviceName}"
                : $"{dto.Notes} (من الجهاز {device.DeviceName})";
            item.CompletedAt = DateTime.Now;
            item.ResultStatus = ComputeResultStatus(item, item.LabOrder?.PatientUser);

            if (item.LabOrder != null)
            {
                // استثناء العنصر الحالي من الاستعلام لأن تغييره لم يُحفظ بعد في القاعدة
                var allDone = await _context.LabOrderItems
                    .Where(i => i.LabOrderID == item.LabOrder.LabOrderID && i.LabOrderItemID != item.LabOrderItemID)
                    .AllAsync(i => i.ResultStatus != "Pending");
                if (allDone)
                {
                    item.LabOrder.Status = "Completed";
                    item.LabOrder.CompletedAt = DateTime.Now;
                    item.LabOrder.VerificationQRCode = $"CLINICPRO-LAB-{item.LabOrder.LabOrderID}-{item.LabOrder.PatientUserID}-{DateTime.UtcNow.Ticks}";
                }
                else if (item.LabOrder.Status == "Requested")
                {
                    item.LabOrder.Status = "InProgress";
                }
            }

            await _context.SaveChangesAsync();
            await AuditAsync("LabDeviceCapture", "LabOrderItem", item.LabOrderItemID, $"استلام نتيجة من الجهاز {device.DeviceName} لعنصر {item.LabOrderItemID}");
            return Ok(ApiResponse<object>.Ok(item, $"تم استلام نتيجة التحليل من الجهاز {device.DeviceName} بنجاح"));
        }

        // ============================================================
        //  أدوات مساعدة
        // ============================================================

        /// <summary>
        /// احتساب الحالة التلقائية للنتيجة (Normal/High/Low) مع مراعاة الجنس والعمر
        /// </summary>
        private static string ComputeResultStatus(LabOrderItem item, User? patient)
        {
            if (item.LabTest == null || string.IsNullOrWhiteSpace(item.ResultValue))
                return "Pending";

            if (!decimal.TryParse(item.ResultValue, out decimal val))
            {
                // نتائج نوعية (مثل إيجابي/سلبي) لا تُقارن عددياً
                var low = item.ResultValue.Trim().ToLowerInvariant();
                if (low is "positive" or "reactive" or "present")
                    return "High";
                return "Normal";
            }

            var ranges = item.LabTest.ReferenceRanges;
            if (ranges.Count == 0)
                return "Normal";

            // اختيار النطاق الأنسب حسب جنس المريض وعمره
            var age = patient?.PatientProfile?.DateOfBirth.HasValue == true
                ? (int)((DateTime.Now - patient.PatientProfile.DateOfBirth.Value).TotalDays / 365.25)
                : 0;

            var gender = patient?.PatientProfile?.Gender;

            var range = ranges
                .Where(r => r.Gender == "All" || (gender != null && r.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)))
                .Where(r => age >= r.MinAge && age <= r.MaxAge)
                .OrderBy(r => r.Gender == gender ? 0 : 1)
                .FirstOrDefault()
                ?? ranges.OrderBy(r => r.Gender == "All" ? 0 : 1).FirstOrDefault();

            if (range == null)
                return "Normal";

            if (val < range.NormalMin) return "Low";
            if (val > range.NormalMax) return "High";
            return "Normal";
        }

        private async Task AuditAsync(string action, string entityType, int entityId, string details)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = action,
                EntityType = entityType,
                EntityID = entityId,
                UserID = GetUserId(),
                Details = details,
                Timestamp = DateTime.Now
            });
        }
    }
}
