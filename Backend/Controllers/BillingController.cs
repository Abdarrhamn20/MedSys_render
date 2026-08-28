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
    public class BillingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BillingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/billing/invoices
        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices([FromQuery] string? status, [FromQuery] string? type, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Invoices
                .Include(i => i.PatientUser)
                .Include(i => i.Appointment)
                    .ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
                .Include(i => i.DispenseRecord)
                    .ThenInclude(d => d!.Prescription)
                .Include(i => i.LabOrder)
                    .ThenInclude(lo => lo!.Items).ThenInclude(li => li!.LabTest)
                .Include(i => i.RadiologyOrder)
                .AsQueryable();

            // Filter by role
            if (role == "Patient")
            {
                query = query.Where(i => i.PatientUserID == userId);
            }
            else if (role == "Pharmacist")
            {
                // الصيدلاني يرى فواتير صرف الأدوية فقط
                query = query.Where(i => i.InvoiceType == "Pharmacy");
            }
            else if (role is "Doctor" or "Cashier")
            {
                if (role == "Doctor")
                {
                    var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                    query = query.Where(i => i.Appointment != null && i.Appointment.DoctorID == doctorId);
                }
                // الكاشير يرى كافة الفواتير لتحصيلها
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            // Filter by type
            if (!string.IsNullOrEmpty(type))
                query = query.Where(i => i.InvoiceType == type);

            var totalCount = await query.CountAsync();

            var invoices = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new
                {
                    i.InvoiceID,
                    i.PatientUserID,
                    PatientName = i.PatientUser.FullName,
                    i.AppointmentID,
                    DoctorName = i.Appointment != null ? i.Appointment.Doctor.User.FullName : null,
                    AppointmentDate = i.Appointment != null ? (DateTime?)i.Appointment.AppointmentDate : null,
                    i.DispenseRecordID,
                    MedicationName = i.DispenseRecord != null ? i.DispenseRecord.Prescription.MedicationName : null,
                    LabOrderID = i.LabOrderID,
                    LabTestsSummary = i.LabOrder != null && i.LabOrder.Items.Any()
                        ? string.Join(", ", i.LabOrder.Items.Where(li => li.LabTest != null).Select(li => li.LabTest!.TestName))
                        : null,
                    RadiologyOrderID = i.RadiologyOrderID,
                    RadiologySummary = i.RadiologyOrder != null
                        ? $"{i.RadiologyOrder.Modality} - {i.RadiologyOrder.BodyPart}"
                        : null,
                    i.InvoiceType,
                    i.Amount,
                    i.Tax,
                    i.Discount,
                    i.TotalAmount,
                    i.Status,
                    i.PaymentMethod,
                    i.TransactionReference,
                    i.CreatedAt,
                    i.PaidAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = invoices.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/billing/invoices/5
        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var invoice = await _context.Invoices
                .Include(i => i.PatientUser)
                .Include(i => i.Appointment)
                    .ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
                .Include(i => i.DispenseRecord)
                    .ThenInclude(d => d!.Prescription)
                .Include(i => i.LabOrder)
                    .ThenInclude(lo => lo!.Items).ThenInclude(li => li!.LabTest)
                .Include(i => i.RadiologyOrder)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
                return NotFound(ApiResponse.Fail("الفاتورة غير موجودة"));

            // Check permissions
            if (role == "Patient" && invoice.PatientUserID != userId)
                return Forbid();

            if (role == "Doctor" && (invoice.Appointment == null || invoice.Appointment.Doctor.UserID != userId))
                return Forbid();

            // الصيدلاني يطلع على فواتير صرف الأدوية فقط
            if (role == "Pharmacist" && invoice.InvoiceType != "Pharmacy")
                return Forbid();

            // باقي الأدوار (فني مختبر، أخصائي أشعة، ...) لا حق لها في الاطلاع على الفواتير
            if (role is not ("Admin" or "Cashier" or "Patient" or "Doctor" or "Pharmacist"))
                return Forbid();

            var result = new
            {
                invoice.InvoiceID,
                invoice.PatientUserID,
                PatientName = invoice.PatientUser.FullName,
                PatientPhone = invoice.PatientUser.Phone,
                invoice.AppointmentID,
                DoctorName = invoice.Appointment != null ? invoice.Appointment.Doctor.User.FullName : null,
                DoctorSpecialty = invoice.Appointment != null ? invoice.Appointment.Doctor.Specialty : null,
                AppointmentDate = invoice.Appointment != null ? (DateTime?)invoice.Appointment.AppointmentDate : null,
                AppointmentTime = invoice.Appointment != null ? (TimeSpan?)invoice.Appointment.AppointmentTime : null,
                invoice.DispenseRecordID,
                MedicationName = invoice.DispenseRecord != null ? invoice.DispenseRecord.Prescription.MedicationName : null,
                QuantityDispensed = invoice.DispenseRecord != null ? (int?)invoice.DispenseRecord.QuantityDispensed : null,
                invoice.LabOrderID,
                LabOrderStatus = invoice.LabOrder?.Status,
                LabTests = invoice.LabOrder?.Items?.Select(li => new
                {
                    li.LabTestID,
                    TestName = li.LabTest?.TestName,
                    li.LabTest?.Code,
                    li.LabTest?.Price,
                    li.ResultStatus,
                    li.ResultValue
                }).ToList(),
                invoice.RadiologyOrderID,
                RadiologyModality = invoice.RadiologyOrder?.Modality,
                RadiologyBodyPart = invoice.RadiologyOrder?.BodyPart,
                RadiologyStatus = invoice.RadiologyOrder?.Status,
                invoice.InvoiceType,
                invoice.Amount,
                invoice.Tax,
                invoice.Discount,
                invoice.TotalAmount,
                invoice.Status,
                invoice.PaymentMethod,
                invoice.TransactionReference,
                invoice.CreatedAt,
                invoice.PaidAt
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/billing/invoices/{id}/pay
        [HttpPost("invoices/{id}/pay")]
        [Authorize(Roles = "Admin,Pharmacist,Patient,Cashier")]
        public async Task<IActionResult> PayWithCard(int id, [FromBody] CardPaymentDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var invoice = await _context.Invoices
                .Include(i => i.Appointment)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
                return NotFound(ApiResponse.Fail("الفاتورة غير موجودة"));

            if (role == "Patient" && invoice.PatientUserID != userId)
                return Forbid();

            // الصيدلاني يدفع فواتير الصيدلية فقط
            if (role == "Pharmacist" && invoice.InvoiceType != "Pharmacy")
                return Forbid();

            if (invoice.Status == "Paid")
                return BadRequest(ApiResponse.Fail("الفاتورة مدفوعة بالفعل"));

            // ضابط مالي: لا يُقبل تحصيل في فترة مالية مقفلة (الإقفال الشهري)
            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, DateTime.Now);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            // Mock Card Validation
            if (string.IsNullOrEmpty(dto.CardNumber) || dto.CardNumber.Length < 16)
                return BadRequest(ApiResponse.Fail("رقم البطاقة غير صالح، يجب أن يتكون من 16 خانة"));

            if (string.IsNullOrEmpty(dto.Cvc) || dto.Cvc.Length < 3)
                return BadRequest(ApiResponse.Fail("رمز الأمان (CVC) غير صالح"));

            // Perform payment
            invoice.Status = "Paid";
            invoice.PaymentMethod = "Card";
            invoice.PaidAt = DateTime.Now;
            invoice.TransactionReference = "TXN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Confirm appointment if linked
            if (invoice.Appointment != null)
            {
                invoice.Appointment.Status = "Confirmed";
            }

            // Audit log
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "InvoicePaid",
                EntityType = "Invoice",
                EntityID = invoice.InvoiceID,
                UserID = userId,
                Details = $"تم دفع الفاتورة رقم #{invoice.InvoiceID} إلكترونياً بقيمة {invoice.TotalAmount} دينار ليبي. مرجع المعاملة: {invoice.TransactionReference}",
                Timestamp = DateTime.Now
            });

            await JournalAutoHelper.CreateInvoiceCollectionVoucherAsync(_context, userId, invoice);
            await JournalAutoHelper.CreateInvoiceCollectionEntryAsync(_context, userId, invoice);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { transactionRef = invoice.TransactionReference }, "تمت عملية الدفع الإلكتروني بنجاح وتأكيد الخدمة"));
        }

        // POST: api/billing/invoices/{id}/pay-cash
        [HttpPost("invoices/{id}/pay-cash")]
        [Authorize(Roles = "Admin,Pharmacist,Patient,Cashier")]
        public async Task<IActionResult> PayWithCash(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var invoice = await _context.Invoices
                .Include(i => i.Appointment)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
                return NotFound(ApiResponse.Fail("الفاتورة غير موجودة"));

            if (invoice.Status == "Paid")
                return BadRequest(ApiResponse.Fail("الفاتورة مدفوعة بالفعل"));

            if (role == "Patient" && invoice.PatientUserID != userId)
                return Forbid();

            // ضابط مالي: لا يُقبل تحصيل في فترة مالية مقفلة (الإقفال الشهري)
            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, DateTime.Now);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            // الصيدلاني يدفع فواتير الصيدلية فقط
            if (role == "Pharmacist" && invoice.InvoiceType != "Pharmacy")
                return Forbid();

            invoice.Status = "Paid";
            invoice.PaymentMethod = "Cash";
            invoice.PaidAt = DateTime.Now;
            invoice.TransactionReference = "CASH-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Confirm appointment if linked
            if (invoice.Appointment != null)
            {
                invoice.Appointment.Status = "Confirmed";
            }

            // Audit log
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "InvoicePaidCash",
                EntityType = "Invoice",
                EntityID = invoice.InvoiceID,
                UserID = userId,
                Details = $"تم تحصيل الفاتورة رقم #{invoice.InvoiceID} نقداً بقيمة {invoice.TotalAmount} دينار ليبي في الاستقبال.",
                Timestamp = DateTime.Now
            });

            await JournalAutoHelper.CreateInvoiceCollectionVoucherAsync(_context, userId, invoice);
            await JournalAutoHelper.CreateInvoiceCollectionEntryAsync(_context, userId, invoice);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تسجيل سداد الفاتورة نقداً بنجاح وتحديث حالة الخدمة"));
        }

        // GET: api/billing/stats
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBillingStats()
        {
            var totalRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid")
                .SumAsync(i => i.TotalAmount);

            var pendingRevenue = await _context.Invoices
                .Where(i => i.Status == "Unpaid")
                .SumAsync(i => i.TotalAmount);

            var paidCount = await _context.Invoices.CountAsync(i => i.Status == "Paid");
            var unpaidCount = await _context.Invoices.CountAsync(i => i.Status == "Unpaid");

            var cashRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.PaymentMethod == "Cash")
                .SumAsync(i => i.TotalAmount);

            var cardRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.PaymentMethod == "Card")
                .SumAsync(i => i.TotalAmount);

            var consultationRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceType == "Consultation")
                .SumAsync(i => i.TotalAmount);

            var pharmacyRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceType == "Pharmacy")
                .SumAsync(i => i.TotalAmount);

            var laboratoryRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceType == "Laboratory")
                .SumAsync(i => i.TotalAmount);

            var radiologyRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceType == "Radiology")
                .SumAsync(i => i.TotalAmount);

            var inpatientRevenue = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.InvoiceType == "Inpatient")
                .SumAsync(i => i.TotalAmount);

            // Last 10 invoices
            var recentInvoices = await _context.Invoices
                .Include(i => i.PatientUser)
                .OrderByDescending(i => i.CreatedAt)
                .Take(10)
                .Select(i => new
                {
                    i.InvoiceID,
                    PatientName = i.PatientUser.FullName,
                    i.InvoiceType,
                    i.TotalAmount,
                    i.Status,
                    i.PaymentMethod,
                    i.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                totalRevenue,
                pendingRevenue,
                paidCount,
                unpaidCount,
                cashRevenue,
                cardRevenue,
                consultationRevenue,
                pharmacyRevenue,
                laboratoryRevenue,
                radiologyRevenue,
                inpatientRevenue,
                recentInvoices
            }));
        }
    }
}
