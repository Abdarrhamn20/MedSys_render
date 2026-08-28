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
    public class CommissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return JwtHelper.GetUserIdFromClaims(User);
        }

        private string GetUserRole()
        {
            return JwtHelper.GetUserRoleFromClaims(User);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCommissions()
        {
            var list = await _context.DoctorCommissions
                .Include(c => c.Doctor)
                .Select(c => new
                {
                    c.CommissionID,
                    c.DoctorID,
                    DoctorName = c.Doctor != null ? c.Doctor.FullName : "عام",
                    c.Specialty,
                    c.CommissionType,
                    c.Value,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(list, "قائمة نسب العمولات للأطباء"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetCommission([FromBody] SetCommissionDTO dto)
        {
            if (dto.CommissionType != "FixedAmount" && dto.CommissionType != "Percentage")
            {
                return BadRequest(ApiResponse.Fail("نوع العمولة غير صالح. القيم المسموحة: FixedAmount أو Percentage."));
            }

            if (dto.Value < 0 || (dto.CommissionType == "Percentage" && dto.Value > 100))
            {
                return BadRequest(ApiResponse.Fail("قيمة العمولة غير صالحة. يجب ألا تقل عن صفر ولا تزيد عن 100% لنسبة العمولة."));
            }

            var doctor = await _context.Users.FirstOrDefaultAsync(u => u.UserID == dto.DoctorID && u.Role == "Doctor");
            if (doctor == null)
            {
                return BadRequest(ApiResponse.Fail("الطبيب المحدد غير موجود."));
            }

            var commission = await _context.DoctorCommissions
                .FirstOrDefaultAsync(c => c.DoctorID == dto.DoctorID);

            if (commission == null)
            {
                commission = new DoctorCommission
                {
                    DoctorID = dto.DoctorID,
                    Specialty = dto.Specialty,
                    CommissionType = dto.CommissionType,
                    Value = dto.Value,
                    CreatedAt = DateTime.Now
                };
                _context.DoctorCommissions.Add(commission);
            }
            else
            {
                commission.CommissionType = dto.CommissionType;
                commission.Value = dto.Value;
                commission.Specialty = dto.Specialty;
            }

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(commission, "تم حفظ نسبة الطبيب بنجاح"));
        }

        [HttpGet("doctor/{doctorId}/ledger")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetDoctorLedger(int doctorId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var currentUserId = GetUserId();
            var currentRole = GetUserRole();

            // Doctors can only view their own financial ledger
            if (currentRole == "Doctor")
            {
                doctorId = currentUserId;
            }

            var doctorUser = await _context.Users
                .Include(u => u.DoctorProfile)
                .FirstOrDefaultAsync(u => u.UserID == doctorId || (u.DoctorProfile != null && u.DoctorProfile.DoctorID == doctorId));

            if (doctorUser == null)
            {
                return NotFound(ApiResponse.Fail("حساب الطبيب غير موجود."));
            }

            var docUserId = doctorUser.UserID;
            var docProfileId = doctorUser.DoctorProfile?.DoctorID ?? 0;

            var start = fromDate?.Date ?? DateTime.Today.AddDays(-30);
            var end = toDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.Today.AddDays(1).AddTicks(-1);

            var invoices = await _context.Invoices
                .Include(i => i.PatientUser)
                .Include(i => i.Appointment)
                .Where(i => i.Status == "Paid" && 
                            (i.DoctorID == docUserId || (docProfileId > 0 && i.DoctorID == docProfileId) || (i.Appointment != null && i.Appointment.DoctorID == docProfileId)) && 
                            i.CreatedAt >= start && i.CreatedAt <= end)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var summary = new DoctorLedgerSummaryDTO
            {
                DoctorID = doctorUser.UserID,
                DoctorName = doctorUser.FullName,
                Specialty = doctorUser.DoctorProfile?.Specialty ?? "طبيب عام",
                TotalRevenue = invoices.Sum(i => i.TotalAmount),
                DoctorTotalEarnings = invoices.Sum(i => i.DoctorShare),
                ClinicTotalShare = invoices.Sum(i => i.ClinicShare),
                TotalConsultations = invoices.Count,
                Transactions = invoices.Select(i => new DoctorLedgerItemDTO
                {
                    InvoiceID = i.InvoiceID,
                    AppointmentID = i.AppointmentID ?? 0,
                    PatientName = i.PatientUser != null ? i.PatientUser.FullName : "مريض عام",
                    TotalAmount = i.TotalAmount,
                    DoctorShare = i.DoctorShare,
                    ClinicShare = i.ClinicShare,
                    PaymentMethod = i.PaymentMethod ?? "Cash",
                    Date = i.CreatedAt
                }).ToList()
            };

            return Ok(ApiResponse<DoctorLedgerSummaryDTO>.Ok(summary, "كشف حساب أرباح الطبيب"));
        }

        [HttpGet("daily-cash-report")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> GetDailyCashReport([FromQuery] DateTime? date)
        {
            var targetDate = date?.Date ?? DateTime.Today;
            var start = targetDate;
            var end = targetDate.AddDays(1).AddTicks(-1);

            var invoices = await _context.Invoices
                .Include(i => i.PatientUser)
                .Include(i => i.Doctor)
                .Where(i => i.Status == "Paid" && i.CreatedAt >= start && i.CreatedAt <= end)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var report = new DailyCashReportSummaryDTO
            {
                Date = targetDate,
                TotalCash = invoices.Where(i => i.PaymentMethod == "Cash").Sum(i => i.TotalAmount),
                TotalPOS = invoices.Where(i => i.PaymentMethod == "POS").Sum(i => i.TotalAmount),
                TotalOnline = invoices.Where(i => i.PaymentMethod == "Card" || i.PaymentMethod == "Online").Sum(i => i.TotalAmount),
                GrandTotal = invoices.Sum(i => i.TotalAmount),
                TotalInvoices = invoices.Count,
                Payments = invoices.Select(i => new DailyCashItemDTO
                {
                    InvoiceID = i.InvoiceID,
                    InvoiceType = i.InvoiceType,
                    PatientName = i.PatientUser != null ? i.PatientUser.FullName : "مريض عام",
                    DoctorName = i.Doctor != null ? i.Doctor.FullName : "العيادة",
                    TotalAmount = i.TotalAmount,
                    PaymentMethod = i.PaymentMethod ?? "Cash",
                    Time = i.CreatedAt
                }).ToList()
            };

            return Ok(ApiResponse<DailyCashReportSummaryDTO>.Ok(report, "تقرير إغلاق الخزينة اليومي"));
        }

        [HttpPost("express-booking")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<IActionResult> ProcessExpressBooking([FromBody] ExpressBookingDTO dto)
        {
            // موظف الاستقبال يصدر تذكرة بحجز مؤكد وفاتورة غير مدفوعة (السداد لدى الكاشير)
            var isReceptionist = User.IsInRole("Receptionist");

            if (string.IsNullOrWhiteSpace(dto.PatientName))
            {
                return BadRequest(ApiResponse.Fail("اسم المريض مطلوب."));
            }

            var doctor = await _context.Users
                .Include(u => u.DoctorProfile)
                .FirstOrDefaultAsync(u => u.UserID == dto.DoctorID && u.Role == "Doctor");

            if (doctor == null)
            {
                return BadRequest(ApiResponse.Fail("الطبيب المحدد غير موجود."));
            }

            // 1. Find or create patient user
            var phone = string.IsNullOrWhiteSpace(dto.PatientPhone) ? "0900000000" : dto.PatientPhone;
            var patientUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == dto.PatientName || (phone != "0900000000" && u.Phone == phone));

            if (patientUser == null)
            {
                patientUser = new User
                {
                    FullName = dto.PatientName,
                    Email = "walkin_" + Guid.NewGuid().ToString().Substring(0, 8) + "@clinic.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Patient123!"),
                    Phone = dto.PatientPhone ?? "0900000000",
                    Role = "Patient",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(patientUser);
                await _context.SaveChangesAsync();

                var patientProfile = new PatientProfile
                {
                    UserID = patientUser.UserID,
                    Gender = dto.Gender ?? "ذكر",
                    FileNumber = await FileNumberHelper.GenerateNextAsync(_context)
                };
                _context.PatientProfiles.Add(patientProfile);
                await _context.SaveChangesAsync();
            }

            var pProfile = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.UserID == patientUser.UserID);

            // 2. Calculate Today Queue Number for this Doctor
            var today = DateTime.Today;
            var queueCount = await _context.Appointments
                .Where(a => a.DoctorID == (doctor.DoctorProfile != null ? doctor.DoctorProfile.DoctorID : doctor.UserID) && a.AppointmentDate.Date == today)
                .CountAsync();
            var queueNum = queueCount + 1;

            // Priority default = Normal (ID 1)
            var defaultPriority = await _context.Priorities.FirstOrDefaultAsync(p => p.LevelNameAr == "عادي" || p.LevelName == "Normal")
                ?? await _context.Priorities.FirstOrDefaultAsync();

            var docProfileId = doctor.DoctorProfile != null ? doctor.DoctorProfile.DoctorID : doctor.UserID;

            var appointment = new Appointment
            {
                PatientID = pProfile != null ? pProfile.PatientID : 1,
                DoctorID = docProfileId,
                PriorityID = defaultPriority != null ? defaultPriority.PriorityID : 1,
                AppointmentDate = today,
                AppointmentTime = DateTime.Now.TimeOfDay,
                Status = "Confirmed",
                AppointmentType = "WalkIn",
                QueueNumber = queueNum,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes ?? "حجز سريع Walk-in",
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // 3. Calculate Doctor & Clinic Share
            var commission = await _context.DoctorCommissions
                .FirstOrDefaultAsync(c => c.DoctorID == doctor.UserID);

            decimal fee = dto.ConsultationFee > 0 ? dto.ConsultationFee : (doctor.DoctorProfile?.ConsultationFee ?? 50.00m);
            decimal doctorShare = 0.00m;
            decimal clinicShare = 0.00m;

            if (commission != null)
            {
                if (commission.CommissionType == "FixedAmount")
                {
                    doctorShare = Math.Min(commission.Value, fee);
                    clinicShare = Math.Max(fee - doctorShare, 0);
                }
                else // Percentage
                {
                    decimal pct = Math.Clamp(commission.Value, 0, 100);
                    doctorShare = Math.Round((fee * pct) / 100m, 2);
                    clinicShare = fee - doctorShare;
                }
            }
            else
            {
                // Default 50% split if no custom commission set
                doctorShare = Math.Round(fee * 0.50m, 2);
                clinicShare = fee - doctorShare;
            }

            // 4. Create Paid Invoice
            var invoice = new Invoice
            {
                PatientUserID = patientUser.UserID,
                AppointmentID = appointment.AppID,
                InvoiceType = "Consultation",
                Amount = fee,
                Tax = 0,
                Discount = 0,
                TotalAmount = fee,
                Status = isReceptionist ? "Unpaid" : "Paid",
                PaymentMethod = dto.PaymentMethod,
                PaidAt = isReceptionist ? (DateTime?)null : DateTime.Now,
                DoctorID = doctor.UserID,
                DoctorShare = doctorShare,
                ClinicShare = clinicShare,
                DoctorCommissionID = commission?.CommissionID,
                CreatedAt = DateTime.Now
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // قيد محاسبي تلقائي للفاتورة المدفوعة فوراً (الحجز السريع النقدي) + سند قبض مرتبط بالحجز
            if (!isReceptionist)
            {
                await JournalAutoHelper.CreateInvoiceCollectionVoucherAsync(_context, JwtHelper.GetUserIdFromClaims(User), invoice);
                await JournalAutoHelper.CreateInvoiceCollectionEntryAsync(_context, JwtHelper.GetUserIdFromClaims(User), invoice);
                await _context.SaveChangesAsync();
            }

            // Return thermal receipt print ready object
            var receipt = new
            {
                AppointmentID = appointment.AppID,
                InvoiceID = invoice.InvoiceID,
                QueueNumber = queueNum,
                PatientName = patientUser.FullName,
                DoctorName = doctor.FullName,
                Specialty = doctor.DoctorProfile?.Specialty ?? "طب عام",
                Fee = fee,
                DoctorShare = doctorShare,
                ClinicShare = clinicShare,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = isReceptionist ? "Unpaid" : "Paid",
                Date = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Time = DateTime.Now.ToString("hh:mm tt"),
                QRCodeData = $"CLINICPRO-TICKET-{appointment.AppID}-{queueNum}-{patientUser.FullName}"
            };

            var msg = isReceptionist
                ? "تم إصدار تذكرة الدخول بنجاح — الفاتورة معلّقة بانتظار السداد لدى الكاشير"
                : "تم الحجز السريع وإصدار الفاتورة والتذكرة بنجاح";

            return Ok(ApiResponse<object>.Ok(receipt, msg));
        }
    }
}
