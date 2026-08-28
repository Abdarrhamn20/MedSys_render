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
    public class PharmacyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PharmacyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================================
        //  MEDICATIONS (إدارة مخزون الأدوية)
        // =============================================

        // GET: api/pharmacy/medications
        [HttpGet("medications")]
        [Authorize(Roles = "Admin,Pharmacist,Doctor")]
        public async Task<IActionResult> GetMedications([FromQuery] string? search, [FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Medications.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Name.Contains(search) || m.NameAr.Contains(search));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(m => m.Category == category);

            var totalCount = await query.CountAsync();

            var medications = await query
                .OrderBy(m => m.NameAr)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MedicationID,
                    m.Name,
                    m.NameAr,
                    m.Category,
                    m.DosageForm,
                    m.Unit,
                    m.QuantityInStock,
                    m.MinStockLevel,
                    m.PurchasePrice,
                    m.SellingPrice,
                    m.Manufacturer,
                    m.ExpiryDate,
                    m.IsActive,
                    IsLowStock = m.QuantityInStock <= m.MinStockLevel,
                    IsExpired = m.ExpiryDate.HasValue && m.ExpiryDate.Value < DateTime.Now
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = medications.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // POST: api/pharmacy/medications
        [HttpPost("medications")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> AddMedication([FromBody] MedicationDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) && string.IsNullOrWhiteSpace(dto.NameAr))
                return BadRequest(ApiResponse.Fail("اسم الدواء مطلوب"));

            if (dto.QuantityInStock < 0 || dto.MinStockLevel < 0)
                return BadRequest(ApiResponse.Fail("الكمية وحد الحد الأدنى لا يمكن أن تكون سالبة"));

            if (dto.PurchasePrice < 0 || dto.SellingPrice < 0)
                return BadRequest(ApiResponse.Fail("الأسعار لا يمكن أن تكون سالبة"));

            var medication = new Medication
            {
                Name = dto.Name,
                NameAr = dto.NameAr,
                Category = dto.Category,
                DosageForm = dto.DosageForm,
                Unit = dto.Unit,
                QuantityInStock = dto.QuantityInStock,
                MinStockLevel = dto.MinStockLevel,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                Manufacturer = dto.Manufacturer,
                ExpiryDate = dto.ExpiryDate,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { medicationId = medication.MedicationID }, "تم إضافة الدواء بنجاح"));
        }

        // PUT: api/pharmacy/medications/5
        [HttpPut("medications/{id}")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> UpdateMedication(int id, [FromBody] UpdateMedicationDTO dto)
        {
            if (dto.QuantityInStock.HasValue && dto.QuantityInStock.Value < 0)
                return BadRequest(ApiResponse.Fail("الكمية لا يمكن أن تكون سالبة"));

            if (dto.PurchasePrice.HasValue && dto.PurchasePrice.Value < 0)
                return BadRequest(ApiResponse.Fail("سعر الشراء لا يمكن أن يكون سالباً"));

            if (dto.SellingPrice.HasValue && dto.SellingPrice.Value < 0)
                return BadRequest(ApiResponse.Fail("سعر البيع لا يمكن أن يكون سالباً"));

            var medication = await _context.Medications.FindAsync(id);
            if (medication == null)
                return NotFound(ApiResponse.Fail("الدواء غير موجود"));

            medication.Name = dto.Name ?? medication.Name;
            medication.NameAr = dto.NameAr ?? medication.NameAr;
            medication.Category = dto.Category ?? medication.Category;
            medication.DosageForm = dto.DosageForm ?? medication.DosageForm;
            medication.Unit = dto.Unit ?? medication.Unit;
            if (dto.QuantityInStock.HasValue)
                medication.QuantityInStock = dto.QuantityInStock.Value;
            if (dto.MinStockLevel.HasValue)
                medication.MinStockLevel = dto.MinStockLevel.Value;
            if (dto.PurchasePrice.HasValue)
                medication.PurchasePrice = dto.PurchasePrice.Value;
            if (dto.SellingPrice.HasValue)
                medication.SellingPrice = dto.SellingPrice.Value;
            medication.Manufacturer = dto.Manufacturer ?? medication.Manufacturer;
            medication.ExpiryDate = dto.ExpiryDate ?? medication.ExpiryDate;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم تحديث بيانات الدواء بنجاح"));
        }

        // DELETE: api/pharmacy/medications/5
        [HttpDelete("medications/{id}")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null)
                return NotFound(ApiResponse.Fail("الدواء غير موجود"));

            medication.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم حذف الدواء بنجاح"));
        }

        // GET: api/pharmacy/medications/categories
        [HttpGet("medications/categories")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Medications
                .Where(m => m.Category != null)
                .Select(m => m.Category)
                .Distinct()
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(categories));
        }

        // GET: api/pharmacy/low-stock
        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetLowStock()
        {
            var lowStock = await _context.Medications
                .Where(m => m.IsActive && m.QuantityInStock <= m.MinStockLevel)
                .OrderBy(m => m.QuantityInStock)
                .Select(m => new
                {
                    m.MedicationID,
                    m.NameAr,
                    m.QuantityInStock,
                    m.MinStockLevel,
                    m.Unit,
                    Deficit = m.MinStockLevel - m.QuantityInStock
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(lowStock));
        }

        // =============================================
        //  DISPENSING (صرف الوصفات)
        // =============================================

        // GET: api/pharmacy/prescriptions/pending
        [HttpGet("prescriptions/pending")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetPendingPrescriptions()
        {
            var prescriptions = await _context.Prescriptions
                .Where(p => p.DispenseStatus == "Pending")
                .Include(p => p.MedicalRecord)
                    .ThenInclude(r => r.Appointment)
                        .ThenInclude(a => a.Patient).ThenInclude(pt => pt.User)
                .Include(p => p.MedicalRecord)
                    .ThenInclude(r => r.Appointment)
                        .ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PrescriptionID,
                    p.MedicationName,
                    p.Dosage,
                    p.Frequency,
                    p.Duration,
                    p.Instructions,
                    p.Quantity,
                    p.DispenseStatus,
                    p.CreatedAt,
                    p.RecordID,
                    PatientName = p.MedicalRecord.Appointment.Patient.User.FullName,
                    PatientPhone = p.MedicalRecord.Appointment.Patient.User.Phone,
                    DoctorName = p.MedicalRecord.Appointment.Doctor.User.FullName,
                    AppointmentDate = p.MedicalRecord.Appointment.AppointmentDate
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(prescriptions));
        }

        // POST: api/pharmacy/dispense
        [HttpPost("dispense")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> DispensePrescription([FromBody] DispenseDTO dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var prescription = await _context.Prescriptions
                .Include(p => p.MedicalRecord)
                    .ThenInclude(r => r.Appointment)
                        .ThenInclude(a => a.Patient)
                .Include(p => p.MedicalRecord)
                    .ThenInclude(r => r.Appointment)
                        .ThenInclude(a => a.Doctor)
                            .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.PrescriptionID == dto.PrescriptionID);

            if (prescription == null)
                return NotFound(ApiResponse.Fail("الوصفة غير موجودة"));

            if (prescription.DispenseStatus == "Dispensed")
                return BadRequest(ApiResponse.Fail("تم صرف هذه الوصفة مسبقاً"));

            // كمية صرف صالحة وإيجابية (تمنع استنزاف/تضخيم المخزون بقيم سالبة أو صفرية)
            if (dto.Quantity <= 0)
                return BadRequest(ApiResponse.Fail("كمية الصرف يجب أن تكون رقماً موجباً"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            // Check medication in stock if MedicationID provided
            Medication? medication = null;
            if (dto.MedicationID.HasValue && dto.MedicationID > 0)
            {
                medication = await _context.Medications.FindAsync(dto.MedicationID);
                if (medication == null)
                    return NotFound(ApiResponse.Fail("الدواء غير موجود في المخزون"));

                if (medication.QuantityInStock < dto.Quantity)
                    return BadRequest(ApiResponse.Fail($"الكمية المتوفرة ({medication.QuantityInStock}) أقل من المطلوبة ({dto.Quantity})"));

                // Deduct from stock
                medication.QuantityInStock -= dto.Quantity;
            }

            // Create dispense record
            var dispense = new DispenseRecord
            {
                PrescriptionID = dto.PrescriptionID,
                MedicationID = dto.MedicationID,
                QuantityDispensed = dto.Quantity,
                TotalPrice = medication != null ? medication.SellingPrice * dto.Quantity : 0,
                DispensedByUserID = userId,
                Status = "Dispensed",
                Notes = dto.Notes,
                DispensedAt = DateTime.Now
            };

            _context.DispenseRecords.Add(dispense);

            // Update prescription status
            prescription.DispenseStatus = "Dispensed";
            if (dto.MedicationID.HasValue)
                prescription.MedicationID = dto.MedicationID;

            // Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "PrescriptionDispensed",
                EntityType = "Prescription",
                EntityID = prescription.PrescriptionID,
                UserID = userId,
                Details = $"تم صرف الوصفة: {prescription.MedicationName} - الكمية: {dto.Quantity}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            // إنشاء فاتورة صيدلية معلقة تلقائياً للمريض
            var invoice = new Invoice
            {
                PatientUserID = prescription.MedicalRecord.Appointment.Patient.UserID,
                DoctorID = prescription.MedicalRecord.Appointment.Doctor?.User?.UserID,
                AppointmentID = prescription.MedicalRecord.Appointment.AppID,
                DispenseRecordID = dispense.DispenseID,
                InvoiceType = "Pharmacy",
                Amount = dispense.TotalPrice,
                Tax = 0.00m,
                Discount = 0.00m,
                TotalAmount = dispense.TotalPrice,
                Status = "Unpaid",
                CreatedAt = DateTime.Now
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                dispenseId = dispense.DispenseID,
                totalPrice = dispense.TotalPrice,
                invoiceId = invoice.InvoiceID
            }, "تم صرف الوصفة بنجاح، وتم إنشاء الفاتورة للمريض"));
        }

        // GET: api/pharmacy/dispense-history
        [HttpGet("dispense-history")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetDispenseHistory([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.DispenseRecords
                .Include(d => d.Prescription)
                .Include(d => d.Medication)
                .Include(d => d.DispensedByUser)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(d => d.DispensedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(d => d.DispensedAt <= to.Value.AddDays(1));

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(d => d.DispensedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.DispenseID,
                    d.PrescriptionID,
                    MedicationName = d.Prescription.MedicationName,
                    MedicationFromStock = d.Medication != null ? d.Medication.NameAr : null,
                    d.QuantityDispensed,
                    d.TotalPrice,
                    d.Status,
                    d.Notes,
                    d.DispensedAt,
                    DispensedBy = d.DispensedByUser.FullName
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = records.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // =============================================
        //  DASHBOARD (لوحة تحكم الصيدلية)
        // =============================================

        // GET: api/pharmacy/dashboard
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetDashboard()
        {
            var today = DateTime.Today;

            var pendingPrescriptions = await _context.Prescriptions.CountAsync(p => p.DispenseStatus == "Pending");
            var dispensedToday = await _context.DispenseRecords.CountAsync(d => d.DispensedAt.Date == today);
            var totalMedications = await _context.Medications.CountAsync(m => m.IsActive);
            var lowStockCount = await _context.Medications.CountAsync(m => m.IsActive && m.QuantityInStock <= m.MinStockLevel);
            var revenueToday = await _context.DispenseRecords.Where(d => d.DispensedAt.Date == today).SumAsync(d => d.TotalPrice);
            var revenueMonth = await _context.DispenseRecords
                .Where(d => d.DispensedAt.Month == today.Month && d.DispensedAt.Year == today.Year)
                .SumAsync(d => d.TotalPrice);

            // أكثر الأدوية طلباً هذا الشهر
            var topMedications = await _context.DispenseRecords
                .Where(d => d.DispensedAt.Month == today.Month && d.DispensedAt.Year == today.Year)
                .GroupBy(d => d.Prescription.MedicationName)
                .Select(g => new { Name = g.Key, Count = g.Sum(x => x.QuantityDispensed) })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            // آخر عمليات الصرف
            var recentDispenses = await _context.DispenseRecords
                .OrderByDescending(d => d.DispensedAt)
                .Take(5)
                .Select(d => new
                {
                    d.DispenseID,
                    MedicationName = d.Prescription.MedicationName,
                    d.QuantityDispensed,
                    d.TotalPrice,
                    d.DispensedAt,
                    DispensedBy = d.DispensedByUser.FullName
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                pendingPrescriptions,
                dispensedToday,
                totalMedications,
                lowStockCount,
                revenueToday,
                revenueMonth,
                topMedications,
                recentDispenses
            }));
        }

        // POST: api/pharmacy/requests
        [HttpPost("requests")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateMedicationRequest([FromBody] MedicationRequestDTO dto)
        {
            if (string.IsNullOrEmpty(dto.MedicationName))
                return BadRequest(ApiResponse.Fail("اسم الدواء مطلوب"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var doctor = await _context.Users.FindAsync(userId);
            if (doctor == null)
                return Unauthorized(ApiResponse.Fail("المستخدم غير موجود"));

            var request = new MedicationRequest
            {
                MedicationName = dto.MedicationName,
                DoctorUserID = userId,
                DoctorName = doctor.FullName,
                Notes = dto.Notes,
                IsResolved = false,
                CreatedAt = DateTime.Now
            };

            _context.MedicationRequests.Add(request);
            await _context.SaveChangesAsync();

            // إضافة سجل تدقيق (Audit Log)
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "MedicationRequestCreated",
                EntityType = "MedicationRequest",
                EntityID = request.RequestID,
                UserID = userId,
                Details = $"طلب الطبيب د. {doctor.FullName} توفير دواء: {dto.MedicationName}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { requestId = request.RequestID }, "تم إرسال طلب توفير الدواء بنجاح للمدير والصيدلاني"));
        }

        // GET: api/pharmacy/requests
        [HttpGet("requests")]
        [Authorize(Roles = "Admin,Pharmacist,Doctor")]
        public async Task<IActionResult> GetMedicationRequests([FromQuery] bool? isResolved, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.MedicationRequests.AsQueryable();

            // إذا كان المستخدم طبيباً، يرى فقط طلباته هو، أما الأدمن والصيدلاني فيرون كل الطلبات
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userRole == "Doctor")
            {
                query = query.Where(r => r.DoctorUserID == userId);
            }

            if (isResolved.HasValue)
            {
                query = query.Where(r => r.IsResolved == isResolved.Value);
            }

            var totalCount = await query.CountAsync();

            var requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    requestId = r.RequestID,
                    r.MedicationName,
                    r.DoctorUserID,
                    r.DoctorName,
                    r.Notes,
                    r.IsResolved,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = requests.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // PUT: api/pharmacy/requests/{id}/resolve
        [HttpPut("requests/{id}/resolve")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> ResolveMedicationRequest(int id)
        {
            var request = await _context.MedicationRequests.FindAsync(id);
            if (request == null)
                return NotFound(ApiResponse.Fail("الطلب غير موجود"));

            if (request.IsResolved)
                return BadRequest(ApiResponse.Fail("الطلب تم حله مسبقاً"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            request.IsResolved = true;

            // إضافة سجل تدقيق
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "MedicationRequestResolved",
                EntityType = "MedicationRequest",
                EntityID = request.RequestID,
                UserID = userId,
                Details = $"تم حل طلب توفير الدواء: {request.MedicationName} بنجاح",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تمييز طلب الدواء كموفر بنجاح"));
        }
    }
}
