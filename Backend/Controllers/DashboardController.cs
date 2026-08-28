using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);
            var today = DateTime.Today;

            object stats;

            if (role == "Admin")
            {
                stats = new
                {
                    doctors = await _context.Users.CountAsync(u => u.Role == "Doctor" && u.IsActive),
                    patients = await _context.Users.CountAsync(u => u.Role == "Patient" && u.IsActive),
                    todayAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate == today),
                    emergencies = await _context.Appointments.CountAsync(a => a.PriorityID == 3 && a.Status != "Completed" && a.Status != "Cancelled"),
                    totalAppointments = await _context.Appointments.CountAsync(),
                    completedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Completed"),
                    pendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
                    totalRecords = await _context.MedicalRecords.CountAsync()
                };
            }
            else if (role == "Doctor")
            {
                var doctorProfile = await _context.DoctorProfiles.FirstOrDefaultAsync(d => d.UserID == userId);
                var doctorId = doctorProfile?.DoctorID ?? 0;

                stats = new
                {
                    todayAppointments = await _context.Appointments.CountAsync(a => a.DoctorID == doctorId && a.AppointmentDate == today && a.Status != "Cancelled"),
                    urgentCases = await _context.Appointments.CountAsync(a => a.DoctorID == doctorId && a.PriorityID >= 2 && a.Status == "Pending"),
                    completedThisMonth = await _context.Appointments.CountAsync(a => a.DoctorID == doctorId && a.Status == "Completed" && a.AppointmentDate.Month == today.Month && a.AppointmentDate.Year == today.Year),
                    totalPatients = await _context.Appointments.Where(a => a.DoctorID == doctorId).Select(a => a.PatientID).Distinct().CountAsync(),
                    pendingAppointments = await _context.Appointments.CountAsync(a => a.DoctorID == doctorId && a.Status == "Pending"),
                    consultationFee = doctorProfile?.ConsultationFee ?? 0,
                    totalRevenue = await _context.Invoices.Where(i => i.Appointment != null && i.Appointment.DoctorID == doctorId && i.Status == "Paid").SumAsync(i => (decimal?)i.TotalAmount) ?? 0,
                    pendingRevenue = await _context.Invoices.Where(i => i.Appointment != null && i.Appointment.DoctorID == doctorId && i.Status == "Unpaid").SumAsync(i => (decimal?)i.TotalAmount) ?? 0
                };
            }
            else if (role == "Receptionist")
            {
                stats = new
                {
                    todayAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate == today && a.Status != "Cancelled"),
                    walkInToday = await _context.Appointments.CountAsync(a => a.AppointmentDate == today && a.AppointmentType == "WalkIn" && a.Status != "Cancelled"),
                    confirmedWaiting = await _context.Appointments.CountAsync(a => a.AppointmentDate == today && a.Status == "Confirmed"),
                    unpaidInvoices = await _context.Invoices.CountAsync(i => i.Status == "Unpaid")
                };
            }
            else if (role == "Cashier")
            {
                stats = new
                {
                    todayCash = await _context.Invoices.Where(i => i.Status == "Paid" && i.PaidAt != null && i.PaidAt.Value.Date == today && i.PaymentMethod == "Cash").SumAsync(i => (decimal?)i.TotalAmount) ?? 0,
                    todayTotal = await _context.Invoices.Where(i => i.Status == "Paid" && i.PaidAt != null && i.PaidAt.Value.Date == today).SumAsync(i => (decimal?)i.TotalAmount) ?? 0,
                    paidInvoicesToday = await _context.Invoices.CountAsync(i => i.Status == "Paid" && i.PaidAt != null && i.PaidAt.Value.Date == today),
                    unpaidInvoices = await _context.Invoices.CountAsync(i => i.Status == "Unpaid")
                };
            }
            else if (role == "Pharmacist")
            {
                stats = new
                {
                    pendingPrescriptions = await _context.Prescriptions.CountAsync(p => p.DispenseStatus == "Pending"),
                    dispensedToday = await _context.DispenseRecords.CountAsync(d => d.DispensedAt.Date == today),
                    lowStockMedications = await _context.Medications.CountAsync(m => m.IsActive && m.QuantityInStock <= m.MinStockLevel),
                    revenueToday = await _context.DispenseRecords.Where(d => d.DispensedAt.Date == today).SumAsync(d => (decimal?)d.TotalPrice) ?? 0,
                    totalMedications = await _context.Medications.CountAsync(m => m.IsActive)
                };
            }
            else if (role == "LabTechnician")
            {
                stats = new
                {
                    pendingOrders = await _context.LabOrders.CountAsync(o => o.Status == "Requested" || o.Status == "InProgress"),
                    completedToday = await _context.LabOrders.CountAsync(o => o.Status == "Completed" && o.CompletedAt != null && o.CompletedAt.Value.Date == today),
                    criticalResults = await _context.LabOrders.CountAsync(o => o.Status == "Completed" && o.ResultStatus == "Critical"),
                    totalTests = await _context.LabTests.CountAsync()
                };
            }
            else if (role == "Radiologist")
            {
                stats = new
                {
                    pendingOrders = await _context.RadiologyOrders.CountAsync(o => o.Status == "Requested"),
                    inProgress = await _context.RadiologyOrders.CountAsync(o => o.Status == "InProgress"),
                    reportedToday = await _context.RadiologyOrders.CountAsync(o => o.Status == "Completed" && o.CompletedAt != null && o.CompletedAt.Value.Date == today),
                    totalOrders = await _context.RadiologyOrders.CountAsync()
                };
            }
            else if (role == "WarehouseKeeper")
            {
                var stockItems = await _context.InventoryItems.Where(i => i.IsActive).ToListAsync();
                var postedLines = await _context.StockMovementItems
                    .Include(i => i.Movement)
                    .Where(i => i.Movement.Status == "Posted")
                    .ToListAsync();

                var quantities = new Dictionary<int, decimal>();
                foreach (var line in postedLines)
                {
                    var sign = line.Movement.MovementType == "In" ? 1m
                             : line.Movement.MovementType == "Out" ? -1m
                             : 0m;
                    if (quantities.TryGetValue(line.ItemID, out var q))
                        quantities[line.ItemID] = q + sign * line.Quantity;
                    else
                        quantities[line.ItemID] = sign * line.Quantity;
                }

                var lowStockItems = stockItems.Count(i => (quantities.TryGetValue(i.ItemID, out var q) ? q : 0) <= i.ReorderLevel);
                var expiringSoon = await _context.InventoryItems.CountAsync(i => i.IsActive && i.ExpiryDate != null && i.ExpiryDate.Value.Date <= today.AddDays(60));
                var movementsToday = await _context.StockMovements.CountAsync(m => m.MovementDate.Date == today);

                stats = new
                {
                    totalItems = stockItems.Count,
                    lowStockItems,
                    expiringSoon,
                    movementsToday
                };
            }
            else // Patient
            {
                var patientProfile = await _context.PatientProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
                var patientId = patientProfile?.PatientID ?? 0;

                stats = new
                {
                    upcomingAppointments = await _context.Appointments.CountAsync(a => a.PatientID == patientId && a.AppointmentDate >= today && a.Status != "Cancelled"),
                    medicalRecords = await _context.MedicalRecords.CountAsync(m => m.Appointment.PatientID == patientId),
                    activePrescriptions = await _context.Prescriptions.CountAsync(p => p.MedicalRecord.Appointment.PatientID == patientId),
                    completedVisits = await _context.Appointments.CountAsync(a => a.PatientID == patientId && a.Status == "Completed")
                };
            }

            return Ok(ApiResponse<object>.Ok(stats));
        }

        // GET: api/dashboard/recent-appointments
        [HttpGet("recent-appointments")]
        public async Task<IActionResult> GetRecentAppointments()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.Appointments
                .Include(a => a.Priority)
                .AsQueryable();

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles
                    .Where(d => d.UserID == userId)
                    .Select(d => d.DoctorID)
                    .FirstOrDefaultAsync();
                query = query.Where(a => a.DoctorID == doctorId);
            }
            else if (role == "Patient")
            {
                var patientId = await _context.PatientProfiles
                    .Where(p => p.UserID == userId)
                    .Select(p => p.PatientID)
                    .FirstOrDefaultAsync();
                query = query.Where(a => a.PatientID == patientId);
            }

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.Priority.Weight)
                .Take(10)
                .Select(a => new
                {
                    a.AppID,
                    PatientName = a.Patient.User.FullName,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    a.TriageScore,
                    PriorityLevel = a.Priority.LevelNameAr,
                    PriorityColor = a.Priority.ColorCode,
                    a.PriorityID
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(appointments));
        }

        // GET: api/dashboard/charts/weekly
        [HttpGet("charts/weekly")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWeeklyChart()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-6);

            var dailyData = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                var count = await _context.Appointments.CountAsync(a => a.AppointmentDate == date);
                var completed = await _context.Appointments.CountAsync(a => a.AppointmentDate == date && a.Status == "Completed");
                dailyData.Add(new
                {
                    date = date.ToString("MM/dd"),
                    dayName = date.ToString("dddd", new System.Globalization.CultureInfo("ar-SA")),
                    total = count,
                    completed
                });
            }

            return Ok(ApiResponse<object>.Ok(dailyData));
        }

        // GET: api/dashboard/charts/priorities
        [HttpGet("charts/priorities")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPriorityDistribution()
        {
            var data = await _context.Appointments
                .GroupBy(a => a.PriorityID)
                .Select(g => new
                {
                    priorityId = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            var priorities = await _context.Priorities.ToListAsync();

            var result = priorities.Select(p => new
            {
                p.PriorityID,
                p.LevelNameAr,
                p.ColorCode,
                count = data.FirstOrDefault(d => d.priorityId == p.PriorityID)?.count ?? 0
            });

            return Ok(ApiResponse<object>.Ok(result));
        }

        // GET: api/dashboard/charts/specialties
        [HttpGet("charts/specialties")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTopSpecialties()
        {
            var data = await _context.Appointments
                .Include(a => a.Doctor)
                .GroupBy(a => a.Doctor.Specialty)
                .Select(g => new
                {
                    specialty = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .Take(6)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(data));
        }

        // GET: api/dashboard/charts/doctors-performance
        [HttpGet("charts/doctors-performance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDoctorsPerformance()
        {
            var data = await _context.DoctorProfiles
                .Include(d => d.User)
                .Where(d => d.User.IsActive)
                .Select(d => new
                {
                    doctorName = d.User.FullName,
                    specialty = d.Specialty,
                    total = d.Appointments.Count(),
                    completed = d.Appointments.Count(a => a.Status == "Completed"),
                    pending = d.Appointments.Count(a => a.Status == "Pending")
                })
                .OrderByDescending(x => x.total)
                .Take(10)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(data));
        }
    }
}
