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
    public class InpatientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InpatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        //  1. WARDS & BED GRID (خريطة الأقسام والأسرة)
        // ==========================================

        [HttpGet("wards")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetWards()
        {
            var wards = await _context.Wards
                .Where(w => w.IsActive)
                .Select(w => new
                {
                    w.WardID,
                    w.WardName,
                    w.WardNameAr,
                    w.GenderType,
                    w.FloorNumber,
                    TotalRooms = w.Rooms.Count(r => r.IsActive),
                    TotalBeds = w.Rooms.SelectMany(r => r.Beds).Count(),
                    OccupiedBeds = w.Rooms.SelectMany(r => r.Beds).Count(b => b.Status == "Occupied"),
                    VacantBeds = w.Rooms.SelectMany(r => r.Beds).Count(b => b.Status == "Vacant")
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(wards));
        }

        [HttpPost("wards")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateWard([FromBody] CreateWardDTO dto)
        {
            var ward = new Ward
            {
                WardName = dto.WardName,
                WardNameAr = dto.WardNameAr,
                GenderType = dto.GenderType,
                FloorNumber = dto.FloorNumber,
                IsActive = true
            };

            _context.Wards.Add(ward);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(ward, "تم إضافة القسم/الجناح بنجاح"));
        }

        [HttpGet("bed-grid")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetBedGrid()
        {
            var wards = await _context.Wards
                .Where(w => w.IsActive)
                .Select(w => new
                {
                    w.WardID,
                    w.WardNameAr,
                    w.GenderType,
                    w.FloorNumber,
                    Rooms = w.Rooms.Where(r => r.IsActive).Select(r => new
                    {
                        r.RoomID,
                        r.RoomNumber,
                        r.RoomType,
                        r.DailyRate,
                        r.MaxBeds,
                        Beds = r.Beds.Select(b => new
                        {
                            b.BedID,
                            b.BedNumber,
                            b.Status,
                            b.Notes,
                            CurrentAdmission = b.Admissions
                                .Where(a => a.Status == "Active")
                .Select(a => new
                {
                    a.AdmissionID,
                    a.PatientID,
                    PatientUserID = a.Patient.User.UserID,
                    PatientName = a.Patient.User.FullName,
                                    PatientPhone = a.Patient.User.Phone,
                                    PatientBloodType = a.Patient.BloodType,
                                    DoctorName = a.Doctor.User.FullName,
                                    a.AdmissionDate,
                                    a.AdmissionReason
                                })
                                .FirstOrDefault()
                        })
                    })
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(wards));
        }

        [HttpPost("rooms")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDTO dto)
        {
            var wardExists = await _context.Wards.AnyAsync(w => w.WardID == dto.WardID);
            if (!wardExists)
                return NotFound(ApiResponse.Fail("الجناح المحدد غير موجود"));

            var room = new Room
            {
                WardID = dto.WardID,
                RoomNumber = dto.RoomNumber,
                RoomType = dto.RoomType,
                DailyRate = dto.DailyRate,
                MaxBeds = dto.MaxBeds,
                IsActive = true
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(room, "تم إضافة الغرفة بنجاح"));
        }

        [HttpPost("beds")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBed([FromBody] CreateBedDTO dto)
        {
            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomID == dto.RoomID);
            if (!roomExists)
                return NotFound(ApiResponse.Fail("الغرفة المحددة غير موجودة"));

            var bed = new Bed
            {
                RoomID = dto.RoomID,
                BedNumber = dto.BedNumber,
                Status = "Vacant",
                Notes = dto.Notes
            };

            _context.Beds.Add(bed);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(bed, "تم إضافة السرير بنجاح"));
        }

        // ==========================================
        //  2. ADMISSIONS (عمليات الإقامة والتنويم)
        // ==========================================

        [HttpGet("admissions")]
        public async Task<IActionResult> GetAdmissions([FromQuery] string? status = "Active")
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.Admissions
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Bed).ThenInclude(b => b.Room).ThenInclude(r => r.Ward)
                .AsQueryable();

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                query = query.Where(a => a.DoctorID == doctorId);
            }
            else if (role == "Patient")
            {
                var patientId = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                query = query.Where(a => a.PatientID == patientId);
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            var admissions = await query
                .OrderByDescending(a => a.AdmissionDate)
                .Select(a => new
                {
                    a.AdmissionID,
                    a.PatientID,
                    PatientName = a.Patient.User.FullName,
                    PatientPhone = a.Patient.User.Phone,
                    PatientBloodType = a.Patient.BloodType,
                    a.DoctorID,
                    DoctorName = a.Doctor.User.FullName,
                    a.BedID,
                    BedNumber = a.Bed.BedNumber,
                    RoomNumber = a.Bed.Room.RoomNumber,
                    WardNameAr = a.Bed.Room.Ward.WardNameAr,
                    RoomDailyRate = a.Bed.Room.DailyRate,
                    a.AdmissionDate,
                    a.DischargeDate,
                    a.AdmissionReason,
                    a.Status,
                    LogsCount = a.DailyLogs.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(admissions));
        }

        [HttpGet("admissions/{id}")]
        public async Task<IActionResult> GetAdmissionById(int id)
        {
            var admission = await _context.Admissions
                .Where(a => a.AdmissionID == id)
                .Select(a => new
                {
                    a.AdmissionID,
                    a.PatientID,
                    PatientUserID = a.Patient.User.UserID,
                    PatientName = a.Patient.User.FullName,
                    PatientPhone = a.Patient.User.Phone,
                    PatientBloodType = a.Patient.BloodType,
                    PatientAllergies = a.Patient.Allergies,
                    PatientChronicDiseases = a.Patient.ChronicDiseases,
                    a.DoctorID,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    a.BedID,
                    BedNumber = a.Bed.BedNumber,
                    RoomNumber = a.Bed.Room.RoomNumber,
                    RoomType = a.Bed.Room.RoomType,
                    WardNameAr = a.Bed.Room.Ward.WardNameAr,
                    DailyRate = a.Bed.Room.DailyRate,
                    a.AdmissionDate,
                    a.DischargeDate,
                    a.AdmissionReason,
                    a.Status,
                    a.DischargeSummary,
                    DailyLogs = a.DailyLogs.OrderByDescending(l => l.LogDate).Select(l => new
                    {
                        l.LogID,
                        l.LogDate,
                        LoggedByName = l.LoggedByUser.FullName,
                        l.Temperature,
                        l.BloodPressure,
                        l.PulseRate,
                        l.OxygenLevel,
                        l.DoctorNotes,
                        l.NursingNotes
                    }).ToList(),
                    CareOrders = _context.InpatientCareOrders
                        .Where(o => o.AdmissionID == a.AdmissionID)
                        .OrderByDescending(o => o.ScheduledTime)
                        .Select(o => new
                        {
                            o.OrderID,
                            o.OrderType,
                            o.OrderDescription,
                            o.Frequency,
                            o.ScheduledTime,
                            o.UnitPrice,
                            o.Status,
                            ExecutionsCount = o.Executions.Count()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (admission == null)
                return NotFound(ApiResponse.Fail("سجل التنويم غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (admission.DoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                if (admission.PatientUserID != userId)
                    return Forbid();
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            return Ok(ApiResponse<object>.Ok(admission));
        }

        [HttpPost("admissions")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreateAdmission([FromBody] CreateAdmissionDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            // الطبيب المسجل يوقّع التنويم باسمه دائماً ولا يختار طبيباً آخر
            if (role == "Doctor")
            {
                var ownDoctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                dto.DoctorID = ownDoctorId;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var bed = await _context.Beds.Include(b => b.Room).FirstOrDefaultAsync(b => b.BedID == dto.BedID);
            if (bed == null)
                return NotFound(ApiResponse.Fail("السرير المحدد غير موجود"));

            if (bed.Status == "Occupied")
                return BadRequest(ApiResponse.Fail("السرير المحدد مشغول بالفعل بمريض آخر"));

            var patientExists = await _context.PatientProfiles.AnyAsync(p => p.PatientID == dto.PatientID);
            if (!patientExists)
                return NotFound(ApiResponse.Fail("الملف الطبي للمريض غير موجود"));

            var doctorExists = await _context.DoctorProfiles.AnyAsync(d => d.DoctorID == dto.DoctorID);
            if (!doctorExists)
                return NotFound(ApiResponse.Fail("الطبيب المحدد غير موجود"));

            // منع تنويم مزدوج لنفس المريض (لا يجوز لمريض إقامتان نشطتان في آن واحد)
            var alreadyAdmitted = await _context.Admissions.AnyAsync(a => a.PatientID == dto.PatientID && a.Status == "Active");
            if (alreadyAdmitted)
                return BadRequest(ApiResponse.Fail("المريض منوّم بالفعل في إقامة نشطة أخرى"));

            var admission = new Admission
            {
                PatientID = dto.PatientID,
                DoctorID = dto.DoctorID,
                BedID = dto.BedID,
                AdmissionDate = DateTime.Now,
                AdmissionReason = dto.AdmissionReason,
                Status = "Active"
            };

            // Update Bed status to Occupied
            bed.Status = "Occupied";

            _context.Admissions.Add(admission);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(ApiResponse<object>.Ok(new { admission.AdmissionID }, "تم تنويم المريض وتسكينه في السرير بنجاح"));
        }

        [HttpPost("admissions/{id}/discharge")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> DischargePatient(int id, [FromBody] DischargeAdmissionDTO dto)
        {
            var admission = await _context.Admissions
                .Include(a => a.Bed).ThenInclude(b => b.Room)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AdmissionID == id);

            if (admission == null)
                return NotFound(ApiResponse.Fail("سجل التنويم غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (admission.DoctorID != doctorId)
                    return Forbid();
            }

            if (admission.Status == "Discharged")
                return BadRequest(ApiResponse.Fail("تم خروج المريض مسبقاً من هذا التنويم"));

            var now = DateTime.Now;
            admission.DischargeDate = now;
            admission.Status = "Discharged";
            admission.DischargeSummary = dto.DischargeSummary;

            // Free the Bed
            if (admission.Bed != null)
            {
                admission.Bed.Status = "Vacant";
            }

            // Calculate staying duration (Minimum 1 day)
            var days = (int)Math.Ceiling((now - admission.AdmissionDate).TotalDays);
            if (days < 1) days = 1;

            var dailyRate = admission.Bed?.Room?.DailyRate ?? 0;
            
            // Sum executed billable care services/medications
            var executedServicesTotal = await _context.InpatientCareOrders
                .Where(o => o.AdmissionID == id && o.Status == "Executed")
                .SumAsync(o => o.UnitPrice);

            var totalInpatientAmount = (days * dailyRate) + executedServicesTotal;

            // Create Invoice for Inpatient stay if rate > 0
            if (totalInpatientAmount > 0)
            {
                var doctorUserID = await _context.DoctorProfiles
                    .Where(d => d.DoctorID == admission.DoctorID)
                    .Select(d => d.UserID)
                    .FirstOrDefaultAsync();

                var invoice = new Invoice
                {
                    PatientUserID = admission.Patient.UserID,
                    DoctorID = doctorUserID > 0 ? (int?)doctorUserID : null,
                    InvoiceType = "Inpatient",
                    Amount = totalInpatientAmount,
                    Tax = 0.00m,
                    Discount = 0.00m,
                    TotalAmount = totalInpatientAmount,
                    Status = "Unpaid",
                    CreatedAt = DateTime.Now
                };
                _context.Invoices.Add(invoice);
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { }, $"تم تسجيل خروج المريض وتفريغ السرير بنجاح. أيام الإقامة: ({days}) يوم، وإجمالي التكلفة شاملة الخدمات: ({totalInpatientAmount} د.ل)."));
        }

        // ==========================================
        //  3. DAILY LOGS & VITAL SIGNS (المتابعة اليومية)
        // ==========================================

        [HttpPost("admissions/{id}/logs")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> AddDailyLog(int id, [FromBody] CreateDailyLogDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var admission = await _context.Admissions.FindAsync(id);
            if (admission == null)
                return NotFound(ApiResponse.Fail("سجل التنويم غير موجود"));

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (admission.DoctorID != doctorId)
                    return Forbid();
            }

            if (admission.Status != "Active")
                return BadRequest(ApiResponse.Fail("لا يمكن إضافة ملاحظات يومية لمريض تم خروجه"));

            var log = new InpatientDailyLog
            {
                AdmissionID = id,
                LoggedByUserID = userId,
                LogDate = DateTime.Now,
                Temperature = dto.Temperature,
                BloodPressure = dto.BloodPressure,
                PulseRate = dto.PulseRate,
                OxygenLevel = dto.OxygenLevel,
                DoctorNotes = dto.DoctorNotes,
                NursingNotes = dto.NursingNotes
            };

            _context.InpatientDailyLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(log, "تم تسجيل التقرير والعلامات الحيوية اليومية بنجاح"));
        }

        // ==========================================
        //  4. CARE ORDERS & NURSING MAR (جدولة وتنفيذ خدمات التمريض)
        // ==========================================

        [HttpPost("orders")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreateCareOrder([FromBody] CreateCareOrderDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var admission = await _context.Admissions.FindAsync(dto.AdmissionID);
            if (admission == null)
                return NotFound(ApiResponse.Fail("سجل التنويم غير موجود"));

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (admission.DoctorID != doctorId)
                    return Forbid();
            }

            if (admission.Status != "Active")
                return BadRequest(ApiResponse.Fail("لا يمكن إضافة أمر جدولة لمريض تم خروجه"));

            if (dto.UnitPrice < 0)
                return BadRequest(ApiResponse.Fail("سعر الخدمة لا يمكن أن يكون سالباً"));

            // إذا تم تمرير HealthServiceID والسعر صفر، نأخذ السعر من كتالوج الخدمات
            var unitPrice = dto.UnitPrice;
            if (dto.HealthServiceID.HasValue && unitPrice == 0)
            {
                var healthService = await _context.HealthServices.FindAsync(dto.HealthServiceID.Value);
                if (healthService != null)
                    unitPrice = healthService.Price;
            }

            var order = new InpatientCareOrder
            {
                AdmissionID = dto.AdmissionID,
                HealthServiceID = dto.HealthServiceID,
                OrderType = dto.OrderType,
                OrderDescription = dto.OrderDescription,
                Frequency = dto.Frequency,
                ScheduledTime = dto.ScheduledTime,
                UnitPrice = unitPrice,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                CreatedByUserID = userId
            };

            _context.InpatientCareOrders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(order, "تم إضافة أمر الرعاية والخدمة المجدولة بنجاح"));
        }

        [HttpGet("admissions/{id}/orders")]
        public async Task<IActionResult> GetCareOrders(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var orders = await _context.InpatientCareOrders
                .Include(o => o.CreatedByUser)
                .Include(o => o.Executions).ThenInclude(e => e.ExecutedByUser)
                .Where(o => o.AdmissionID == id)
                .OrderByDescending(o => o.ScheduledTime)
                .Select(o => new
                {
                    o.OrderID,
                    o.AdmissionID,
                    AdmissionDoctorID = o.Admission.DoctorID,
                    AdmissionPatientID = o.Admission.PatientID,
                    PatientUserID = o.Admission.Patient.User.UserID,
                    o.OrderType,
                    o.OrderDescription,
                    o.Frequency,
                    o.ScheduledTime,
                    o.UnitPrice,
                    o.Status,
                    o.CreatedAt,
                    CreatedByName = o.CreatedByUser.FullName,
                    Executions = o.Executions.OrderByDescending(e => e.ExecutedAt).Select(e => new
                    {
                        e.ExecutionID,
                        e.ExecutedAt,
                        ExecutedByName = e.ExecutedByUser.FullName,
                        e.Status,
                        e.Notes,
                        e.VitalTemperature,
                        e.VitalBloodPressure,
                        e.VitalPulse,
                        e.VitalOxygen
                    }).ToList()
                })
                .ToListAsync();

            if (orders.Count == 0)
                return Ok(ApiResponse<object>.Ok(orders));

            var first = orders[0];
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (first.AdmissionDoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                if (first.PatientUserID != userId)
                    return Forbid();
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            return Ok(ApiResponse<object>.Ok(orders));
        }

        [HttpPost("orders/{orderId}/execute")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> ExecuteCareOrder(int orderId, [FromBody] ExecuteCareOrderDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var order = await _context.InpatientCareOrders
                .Include(o => o.Admission)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
                return NotFound(ApiResponse.Fail("أمر الرعاية غير موجود"));

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (order.Admission.DoctorID != doctorId)
                    return Forbid();
            }

            if (order.Status == "Cancelled")
                return BadRequest(ApiResponse.Fail("لا يمكن تنفيذ أمر ملغي"));

            if (order.Status == "Executed")
                return BadRequest(ApiResponse.Fail("تم تنفيذ هذا الأمر مسبقاً ولا يمكن تنفيذه مرة أخرى"));

            var execution = new InpatientCareExecution
            {
                OrderID = orderId,
                ExecutedByUserID = userId,
                ExecutedAt = DateTime.Now,
                Status = "Executed",
                Notes = dto.Notes,
                VitalTemperature = dto.VitalTemperature,
                VitalBloodPressure = dto.VitalBloodPressure,
                VitalPulse = dto.VitalPulse,
                VitalOxygen = dto.VitalOxygen
            };

            // Update order status
            order.Status = "Executed";

            // If vital signs captured, automatically add to InpatientDailyLog for continuous tracking
            if (!string.IsNullOrEmpty(dto.VitalTemperature) || !string.IsNullOrEmpty(dto.VitalBloodPressure) || !string.IsNullOrEmpty(dto.VitalPulse) || !string.IsNullOrEmpty(dto.VitalOxygen))
            {
                var dailyLog = new InpatientDailyLog
                {
                    AdmissionID = order.AdmissionID,
                    LoggedByUserID = userId,
                    LogDate = DateTime.Now,
                    Temperature = dto.VitalTemperature,
                    BloodPressure = dto.VitalBloodPressure,
                    PulseRate = dto.VitalPulse,
                    OxygenLevel = dto.VitalOxygen,
                    NursingNotes = $"تنفيذ أمر رعاية ({order.OrderDescription}): {dto.Notes}"
                };
                _context.InpatientDailyLogs.Add(dailyLog);
            }

            _context.InpatientCareExecutions.Add(execution);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(execution, "تم توثيق تنفيذ الخدمة/الجرعة التمريضية بنجاح"));
        }

        [HttpGet("nursing-dashboard")]
        public async Task<IActionResult> GetNursingDashboard()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role != "Admin" && role != "Doctor")
                return Forbid();

            var query = _context.InpatientCareOrders
                .Include(o => o.Admission).ThenInclude(a => a.Patient).ThenInclude(p => p.User)
                .Include(o => o.Admission).ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .Include(o => o.Admission).ThenInclude(a => a.Bed).ThenInclude(b => b.Room).ThenInclude(r => r.Ward)
                .Include(o => o.CreatedByUser)
                .Where(o => o.Admission.Status == "Active");

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                query = query.Where(o => o.Admission.DoctorID == doctorId);
            }

            var activeOrders = await query
                .OrderBy(o => o.ScheduledTime)
                .Select(o => new
                {
                    o.OrderID,
                    o.AdmissionID,
                    PatientName = o.Admission.Patient.User.FullName,
                    DoctorName = o.Admission.Doctor.User.FullName,
                    WardNameAr = o.Admission.Bed.Room.Ward.WardNameAr,
                    RoomNumber = o.Admission.Bed.Room.RoomNumber,
                    BedNumber = o.Admission.Bed.BedNumber,
                    o.OrderType,
                    o.OrderDescription,
                    o.Frequency,
                    o.ScheduledTime,
                    o.UnitPrice,
                    o.Status,
                    IsOverdue = o.Status == "Pending" && o.ScheduledTime < DateTime.Now
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(activeOrders));
        }
    }
}
