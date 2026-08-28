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
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISettingsService _settings;

        public AppointmentsController(ApplicationDbContext context, ISettingsService settings)
        {
            _context = context;
            _settings = settings;
        }

        // GET: api/appointments?status=&priority=&date=&page=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? priority, [FromQuery] DateTime? date, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Priority)
                .AsQueryable();

            // Role-based filtering
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

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            if (priority.HasValue)
                query = query.Where(a => a.PriorityID == priority.Value);

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate == date.Value.Date);

            var totalCount = await query.CountAsync();

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.Priority.Weight)
                .ThenBy(a => a.AppointmentTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    a.AppID,
                    a.PatientID,
                    PatientName = a.Patient.User.FullName,
                    a.DoctorID,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    a.TriageScore,
                    a.PriorityID,
                    PriorityLevel = a.Priority.LevelNameAr,
                    PriorityColor = a.Priority.ColorCode,
                    PriorityIcon = a.Priority.Icon,
                    a.AppointmentType,
                    a.Notes,
                    a.CreatedAt,
                    HasMedicalRecord = a.MedicalRecord != null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = appointments.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/appointments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _context.Appointments
                .Where(a => a.AppID == id)
                .Select(a => new
                {
                    a.AppID,
                    a.PatientID,
                    PatientName = a.Patient.User.FullName,
                    PatientPhone = a.Patient.User.Phone,
                    PatientBloodType = a.Patient.BloodType,
                    PatientChronicDiseases = a.Patient.ChronicDiseases,
                    PatientAllergies = a.Patient.Allergies,
                    a.DoctorID,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    a.TriageScore,
                    a.PriorityID,
                    PriorityLevel = a.Priority.LevelNameAr,
                    PriorityColor = a.Priority.ColorCode,
                    a.AppointmentType,
                    a.Notes,
                    a.CancellationReason,
                    a.CreatedAt,
                    HasMedicalRecord = a.MedicalRecord != null,
                    MedicalRecordId = a.MedicalRecord != null ? a.MedicalRecord.RecordID : (int?)null
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
                return NotFound(ApiResponse.Fail("الموعد غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (appointment.DoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                var patientId = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                if (appointment.PatientID != patientId)
                    return Forbid();
            }
            else if (role != "Admin" && role != "Receptionist")
            {
                return Forbid();
            }

            return Ok(ApiResponse<object>.Ok(appointment));
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            int patientId;
            if (role == "Patient")
            {
                var profile = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
                if (profile == null)
                    return BadRequest(ApiResponse.Fail("لم يتم العثور على ملف المريض"));
                patientId = profile.PatientID;
            }
            else
            {
                return BadRequest(ApiResponse.Fail("فقط المرضى يمكنهم حجز المواعيد"));
            }

            // Check doctor exists and is active
            var doctor = await _context.DoctorProfiles.Include(d => d.User).FirstOrDefaultAsync(d => d.DoctorID == dto.DoctorID);
            if (doctor == null || !doctor.User.IsActive)
                return BadRequest(ApiResponse.Fail("الطبيب غير متاح"));

            // === سياسة الحجز (قيم من الإعدادات القابلة للتكوين) ===
            var maxDaysAhead = await _settings.GetIntAsync("MaxBookingDaysAhead", 30);
            if (dto.AppointmentDate.Date < DateTime.Today)
                return BadRequest(ApiResponse.Fail("لا يمكن الحجز بتاريخ في الماضي"));
            if (dto.AppointmentDate.Date > DateTime.Today.AddDays(maxDaysAhead))
                return BadRequest(ApiResponse.Fail($"لا يمكن الحجز قبل أكثر من {maxDaysAhead} يوماً من تاريخ اليوم"));

            // التحقق من يوم عمل الطبيب
            if (!string.IsNullOrWhiteSpace(doctor.AvailableDays))
            {
                var days = doctor.AvailableDays
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(d => d.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                var dayAbbrev = dto.AppointmentDate.DayOfWeek.ToString().Substring(0, 3);
                if (!days.Contains(dayAbbrev))
                    return BadRequest(ApiResponse.Fail("الطبيب لا يعمل في هذا اليوم"));
            }

            // وقت الحجز يجب أن يكون ضمن ساعات عمل الطبيب (للكشف عن بُعد والحضور معاً)
            var wkStart = doctor.WorkStartTime ?? new TimeSpan(9, 0, 0);
            var wkEnd = doctor.WorkEndTime ?? new TimeSpan(17, 0, 0);
            var apptDuration = TimeSpan.FromMinutes(doctor.ConsultationDurationMinutes);
            if (dto.AppointmentTime < wkStart || dto.AppointmentTime + apptDuration > wkEnd)
                return BadRequest(ApiResponse.Fail($"وقت الكشف خارج ساعات عمل الطبيب ({DateTime.Today.Add(wkStart).ToString("hh:mm tt")} - {DateTime.Today.Add(wkEnd).ToString("hh:mm tt")})."));

            // عند الحجز لنفس اليوم، يجب ألا يكون الوقت المختار قد مضى
            if (dto.AppointmentDate.Date == DateTime.Today && dto.AppointmentTime < DateTime.Now.TimeOfDay)
                return BadRequest(ApiResponse.Fail("لا يمكن الحجز في وقت مضى من اليوم الحالي"));

            // سقف الحجوزات المستقبلية النشطة للمريض
            var maxFuture = await _settings.GetIntAsync("MaxFutureAppointmentsPerPatient", 5);
            var futureCount = await _context.Appointments.CountAsync(a =>
                a.PatientID == patientId
                && a.AppointmentDate.Date >= DateTime.Today
                && a.Status != "Cancelled"
                && a.Status != "Completed");
            if (futureCount >= maxFuture)
                return BadRequest(ApiResponse.Fail($"بلغت الحد الأقصى للحجوزات المستقبلية ({maxFuture} مواعيد). يرجى إلغاء أحد المواعيد أو الانتظار حتى اكتمال أحدها."));

            // منع تداخل المواعيد للمريض نفسه في نفس اليوم
            var patientAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientID == patientId && a.AppointmentDate == dto.AppointmentDate.Date && a.Status != "Cancelled")
                .ToListAsync();
            var newStart = dto.AppointmentTime;
            var newEnd = newStart.Add(TimeSpan.FromMinutes(doctor.ConsultationDurationMinutes));
            var overlapsOwnAppointment = patientAppointments.Any(a =>
            {
                var otherEnd = a.AppointmentTime.Add(TimeSpan.FromMinutes(a.Doctor.ConsultationDurationMinutes));
                return a.AppointmentTime < newEnd && otherEnd > newStart;
            });
            if (overlapsOwnAppointment)
                return BadRequest(ApiResponse.Fail("لديك موعد آخر متداخل في نفس التوقيت. يرجى اختيار وقت مختلف."));

            // احسب الفرز والأولوية خادمياً من الأسئلة الفعلية، ولا تُقبل من العميل
            var activeQuestions = await _context.TriageQuestions.Where(q => q.IsActive).ToListAsync();
            var triage = TriageEvaluator.Evaluate(activeQuestions, dto.Answers);
            int priorityId = triage.PriorityId;

            // معاملة Serializable لمنع الحجز المزدوج عند التوازي (check-then-insert آمن)
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.DoctorID == dto.DoctorID
                    && a.AppointmentDate == dto.AppointmentDate.Date
                    && a.AppointmentTime == dto.AppointmentTime
                    && a.Status != "Cancelled");

            if (hasConflict)
            {
                await transaction.RollbackAsync();

                // اقتراح أول فتحة متاحة بديلة في نفس اليوم
                var duration = TimeSpan.FromMinutes(doctor.ConsultationDurationMinutes);
                var buffer = await _settings.GetIntAsync("SlotBufferMinutes", 5);
                var step = duration.Add(TimeSpan.FromMinutes(buffer));
                var startTime = doctor.WorkStartTime ?? new TimeSpan(9, 0, 0);
                var endTime = doctor.WorkEndTime ?? new TimeSpan(17, 0, 0);

                var allBooked = await _context.Appointments
                    .Where(a => a.DoctorID == dto.DoctorID && a.AppointmentDate == dto.AppointmentDate.Date && a.Status != "Cancelled")
                    .Select(a => a.AppointmentTime)
                    .ToListAsync();

                TimeSpan? suggestion = null;
                var t = startTime;
                while (t.Add(duration) <= endTime)
                {
                    var slotEnd = t.Add(duration);
                    var booked = allBooked.Any(b => b < slotEnd && b.Add(duration) > t);
                    if (!booked) { suggestion = t; break; }
                    t = t.Add(step);
                }

                var suggestMsg = suggestion.HasValue
                    ? $" يُقترح الحجز عند الساعة {DateTime.Today.Add(suggestion.Value).ToString("hh:mm tt")}."
                    : "";
                return BadRequest(ApiResponse.Fail("هذا الموعد محجوز مسبقاً. اختر وقتاً آخر." + suggestMsg));
            }

            var appointment = new Appointment
            {
                PatientID = patientId,
                DoctorID = dto.DoctorID,
                AppointmentDate = dto.AppointmentDate.Date,
                AppointmentTime = dto.AppointmentTime,
                AppointmentType = dto.AppointmentType == "WalkIn" ? "WalkIn" : "Online",
                PriorityID = priorityId,
                TriageScore = triage.Score,
                Status = priorityId == 3 ? "Confirmed" : "Pending",
                Notes = dto.Notes,
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(); // To get the AppID

            // Log the action
            var log = new AuditLog
            {
                ActionType = "AppointmentCreated",
                EntityType = "Appointment",
                EntityID = appointment.AppID,
                UserID = userId,
                Details = $"تم حجز موعد جديد مع الطبيب {doctor.User.FullName} (درجة الفرز: {triage.Score})",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(log);

            // حساب عمولة الطبيب وحصة العيادة للكشف العادي (مثل الحجز السريع)
            var commission = await _context.DoctorCommissions
                .FirstOrDefaultAsync(c => c.DoctorID == doctor.UserID);
            decimal doctorShare;
            decimal clinicShare;
            if (commission != null && commission.CommissionType == "FixedAmount")
            {
                doctorShare = Math.Min(commission.Value, doctor.ConsultationFee);
                clinicShare = Math.Max(doctor.ConsultationFee - doctorShare, 0);
            }
            else if (commission != null && commission.CommissionType == "Percentage")
            {
                var pct = Math.Clamp(commission.Value, 0, 100);
                doctorShare = Math.Round((doctor.ConsultationFee * pct) / 100, 2);
                clinicShare = doctor.ConsultationFee - doctorShare;
            }
            else
            {
                var defaultRatio = await _settings.GetDecimalAsync("DefaultCommissionRatio", 50);
                var ratio = Math.Clamp(defaultRatio, 0, 100);
                doctorShare = Math.Round(doctor.ConsultationFee * ratio / 100m, 2);
                clinicShare = doctor.ConsultationFee - doctorShare;
            }

            // إنشاء فاتورة كشف الطبيب تلقائياً
            var invoice = new Invoice
            {
                PatientUserID = userId,
                AppointmentID = appointment.AppID,
                InvoiceType = "Consultation",
                Amount = doctor.ConsultationFee,
                Tax = 0.00m,
                Discount = 0.00m,
                TotalAmount = doctor.ConsultationFee,
                Status = "Unpaid",
                DoctorID = doctor.UserID,
                DoctorShare = doctorShare,
                ClinicShare = clinicShare,
                DoctorCommissionID = commission?.CommissionID,
                CreatedAt = DateTime.Now
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var msg = priorityId == 3
                ? "تم حجز الموعد بنجاح عبر المسار السريع — حالة طوارئ"
                : "تم حجز الموعد بنجاح بقيمة كشف تبلغ " + doctor.ConsultationFee.ToString("0.00") + " دينار ليبي";

            return Ok(ApiResponse<object>.Ok(new { appointmentId = appointment.AppID, invoiceId = invoice.InvoiceID }, msg));
        }

        // PUT: api/appointments/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDTO dto)
        {
            var allowedStatuses = new[] { "Pending", "Confirmed", "InProgress", "Completed", "Cancelled" };
            if (string.IsNullOrWhiteSpace(dto.Status) || !allowedStatuses.Contains(dto.Status))
                return BadRequest(ApiResponse.Fail("حالة موعد غير صالحة"));

            var appointment = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppID == id);
            if (appointment == null)
                return NotFound(ApiResponse.Fail("الموعد غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (appointment.DoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                var patientId = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                if (appointment.PatientID != patientId)
                    return Forbid();
                // المريض يستطيع إلغاء موعده فقط، ولا يغيّر أي حالة أخرى
                if (dto.Status != "Cancelled")
                    return Forbid();

                // نافذة الإلغاء: لا يُسمح للمريض بالإلغاء قبل أقل من CancelWindowHours ساعة من الموعد
                var cancelHours = await _settings.GetIntAsync("CancelWindowHours", 6);
                var appointmentStart = appointment.AppointmentDate.Date.Add(appointment.AppointmentTime);
                if (appointmentStart <= DateTime.Now)
                    return BadRequest(ApiResponse.Fail("لا يمكن إلغاء موعد انتهى أو بدأ بالفعل"));
                if ((appointmentStart - DateTime.Now).TotalHours < cancelHours)
                    return BadRequest(ApiResponse.Fail($"لا يمكن إلغاء الموعد قبل أقل من {cancelHours} ساعات من الموعد. يرجى الاتصال بالعيادة."));

                // سبب الإلغاء إجباري للمريض
                if (string.IsNullOrWhiteSpace(dto.CancellationReason))
                    return BadRequest(ApiResponse.Fail("يرجى إدخال سبب الإلغاء"));
            }
            else if (role != "Admin" && role != "Receptionist")
            {
                return Forbid();
            }

            var oldStatus = appointment.Status;
            appointment.Status = dto.Status;
            if (!string.IsNullOrEmpty(dto.CancellationReason))
                appointment.CancellationReason = dto.CancellationReason;

            // Log the action
            var log = new AuditLog
            {
                ActionType = "StatusChange",
                EntityType = "Appointment",
                EntityID = appointment.AppID,
                UserID = userId,
                Details = $"تم تغيير حالة الموعد من {oldStatus} إلى {dto.Status}",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();

            var statusAr = dto.Status switch
            {
                "Confirmed" => "تم تأكيد الموعد",
                "InProgress" => "الجلسة بدأت",
                "Completed" => "تم إكمال الموعد",
                "Cancelled" => "تم إلغاء الموعد",
                _ => "تم تحديث حالة الموعد"
            };

            return Ok(ApiResponse.Ok(statusAr));
        }

        // DELETE: api/appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppID == id);
            if (appointment == null)
                return NotFound(ApiResponse.Fail("الموعد غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (appointment.DoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                var patientId = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                if (appointment.PatientID != patientId)
                    return Forbid();

                // نافذة الإلغاء: لا يُسمح للمريض بالإلغاء قبل أقل من CancelWindowHours ساعة من الموعد
                var cancelHours = await _settings.GetIntAsync("CancelWindowHours", 6);
                var appointmentStart = appointment.AppointmentDate.Date.Add(appointment.AppointmentTime);
                if (appointmentStart <= DateTime.Now)
                    return BadRequest(ApiResponse.Fail("لا يمكن إلغاء موعد انتهى أو بدأ بالفعل"));
                if ((appointmentStart - DateTime.Now).TotalHours < cancelHours)
                    return BadRequest(ApiResponse.Fail($"لا يمكن إلغاء الموعد قبل أقل من {cancelHours} ساعات من الموعد. يرجى الاتصال بالعيادة."));
            }
            else if (role != "Admin" && role != "Receptionist")
            {
                return Forbid();
            }

            var oldStatus = appointment.Status;
            appointment.Status = "Cancelled";

            // Log the action
            var log = new AuditLog
            {
                ActionType = "StatusChange",
                EntityType = "Appointment",
                EntityID = appointment.AppID,
                UserID = userId,
                Details = $"تم إلغاء الموعد من قبل المستخدم",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم إلغاء الموعد"));
        }

        // GET: api/appointments/policy
        [HttpGet("policy")]
        public async Task<IActionResult> GetBookingPolicy()
        {
            var policy = new
            {
                maxDaysAhead = await _settings.GetIntAsync("MaxBookingDaysAhead", 30),
                cancelWindowHours = await _settings.GetIntAsync("CancelWindowHours", 6),
                maxFutureAppointments = await _settings.GetIntAsync("MaxFutureAppointmentsPerPatient", 5),
                slotBufferMinutes = await _settings.GetIntAsync("SlotBufferMinutes", 5)
            };
            return Ok(ApiResponse<object>.Ok(policy));
        }

        // GET: api/appointments/available-slots?doctorId=1&date=2026-05-10
        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(doctorId);
            if (doctor == null)
                return NotFound(ApiResponse.Fail("الطبيب غير موجود"));

            var maxDaysAhead = await _settings.GetIntAsync("MaxBookingDaysAhead", 30);

            if (date.Date < DateTime.Today)
                return Ok(ApiResponse<object>.Ok(new List<object>(), "لا يمكن اختيار تاريخ في الماضي"));

            if (date.Date > DateTime.Today.AddDays(maxDaysAhead))
                return Ok(ApiResponse<object>.Ok(new List<object>(), $"لا يمكن الحجز قبل أكثر من {maxDaysAhead} يوماً من تاريخ اليوم"));

            // التحقق من يوم عمل الطبيب (AvailableDays بصيغة مثل "Sun,Mon,Tue,Wed,Thu")
            if (!string.IsNullOrWhiteSpace(doctor.AvailableDays))
            {
                var days = doctor.AvailableDays
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(d => d.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var dayAbbrev = date.DayOfWeek.ToString().Substring(0, 3);
                if (!days.Contains(dayAbbrev))
                    return Ok(ApiResponse<object>.Ok(new List<object>(), "الطبيب لا يعمل في هذا اليوم"));
            }

            var startTime = doctor.WorkStartTime ?? new TimeSpan(9, 0, 0);
            var endTime = doctor.WorkEndTime ?? new TimeSpan(17, 0, 0);
            var duration = doctor.ConsultationDurationMinutes;
            var buffer = await _settings.GetIntAsync("SlotBufferMinutes", 5);
            var step = TimeSpan.FromMinutes(duration + buffer);

            // Get booked slots
            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorID == doctorId && a.AppointmentDate == date.Date && a.Status != "Cancelled")
                .Select(a => a.AppointmentTime)
                .ToListAsync();

            // Generate all possible slots
            var availableSlots = new List<object>();
            var availableCount = 0;
            var currentTime = startTime;

            while (currentTime.Add(TimeSpan.FromMinutes(duration)) <= endTime)
            {
                var slotEnd = currentTime.Add(TimeSpan.FromMinutes(duration));
                var isBooked = bookedSlots.Any(b => b < slotEnd && b.Add(TimeSpan.FromMinutes(duration)) > currentTime);
                if (!isBooked) availableCount++;

                availableSlots.Add(new
                {
                    time = currentTime,
                    timeFormatted = DateTime.Today.Add(currentTime).ToString("hh:mm tt"),
                    isAvailable = !isBooked
                });
                currentTime = currentTime.Add(step);
            }

            var msg = availableCount > 0
                ? $"{availableCount} فتحات متاحة في هذا اليوم"
                : "لا توجد فتحات متاحة في هذا اليوم";

            return Ok(ApiResponse<object>.Ok(availableSlots, msg));
        }
    }
}
