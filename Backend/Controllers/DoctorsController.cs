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
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/doctors?specialty=&search=
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? specialty, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var role = JwtHelper.GetUserRoleFromClaims(User);
            var isStaff = role == "Admin" || role == "Doctor" || role == "Pharmacist";

            var query = _context.DoctorProfiles
                .Include(d => d.User)
                .Where(d => d.User.IsActive);

            if (!string.IsNullOrEmpty(specialty))
                query = query.Where(d => d.Specialty.Contains(specialty));

            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.User.FullName.Contains(search) || d.Specialty.Contains(search));

            var totalCount = await query.CountAsync();

            var doctors = await query
                .OrderBy(d => d.User.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.DoctorID,
                    d.UserID,
                    d.User.FullName,
                    Email = isStaff ? d.User.Email : null,
                    Phone = isStaff ? d.User.Phone : null,
                    d.Specialty,
                    LicenseNumber = isStaff ? d.LicenseNumber : null,
                    d.EmergencyReady,
                    d.Bio,
                    d.ImageUrl,
                    d.AvailableDays,
                    d.WorkStartTime,
                    d.WorkEndTime,
                    d.ConsultationDurationMinutes,
                    d.ConsultationFee,
                    AppointmentsCount = d.Appointments.Count()
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = doctors.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/doctors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = JwtHelper.GetUserRoleFromClaims(User);
            var isStaff = role == "Admin" || role == "Doctor" || role == "Pharmacist";

            var doctor = await _context.DoctorProfiles
                .Where(d => d.DoctorID == id)
                .Select(d => new
                {
                    d.DoctorID,
                    d.UserID,
                    d.User.FullName,
                    Email = isStaff ? d.User.Email : null,
                    Phone = isStaff ? d.User.Phone : null,
                    d.Specialty,
                    LicenseNumber = isStaff ? d.LicenseNumber : null,
                    d.EmergencyReady,
                    d.Bio,
                    d.ImageUrl,
                    d.AvailableDays,
                    d.WorkStartTime,
                    d.WorkEndTime,
                    d.ConsultationDurationMinutes,
                    d.ConsultationFee,
                    d.User.IsActive,
                    TotalAppointments = d.Appointments.Count(),
                    CompletedAppointments = d.Appointments.Count(a => a.Status == "Completed")
                })
                .FirstOrDefaultAsync();

            if (doctor == null)
                return NotFound(ApiResponse.Fail("الطبيب غير موجود"));

            return Ok(ApiResponse<object>.Ok(doctor));
        }

        // PUT: api/doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DoctorUpdateDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor == null)
                return NotFound(ApiResponse.Fail("الطبيب غير موجود"));

            // Only admin or the doctor themselves can update
            if (role != "Admin" && doctor.UserID != userId)
                return Forbid();

            doctor.Specialty = dto.Specialty ?? doctor.Specialty;
            doctor.LicenseNumber = dto.LicenseNumber ?? doctor.LicenseNumber;
            doctor.EmergencyReady = dto.EmergencyReady;
            doctor.Bio = dto.Bio ?? doctor.Bio;
            doctor.AvailableDays = dto.AvailableDays ?? doctor.AvailableDays;
            doctor.WorkStartTime = dto.WorkStartTime ?? doctor.WorkStartTime;
            doctor.WorkEndTime = dto.WorkEndTime ?? doctor.WorkEndTime;
            doctor.ConsultationDurationMinutes = dto.ConsultationDurationMinutes > 0
                ? dto.ConsultationDurationMinutes : doctor.ConsultationDurationMinutes;
            doctor.ConsultationFee = dto.ConsultationFee >= 0 ? dto.ConsultationFee : doctor.ConsultationFee;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم تحديث بيانات الطبيب بنجاح"));
        }

        // GET: api/doctors/specialties
        [HttpGet("specialties")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSpecialties()
        {
            var specialties = await _context.DoctorProfiles
                .Where(d => d.User.IsActive)
                .Select(d => d.Specialty)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return Ok(ApiResponse<List<string>>.Ok(specialties));
        }

        // GET: api/doctors/emergency-ready
        [HttpGet("emergency-ready")]
        public async Task<IActionResult> GetEmergencyReady()
        {
            var doctors = await _context.DoctorProfiles
                .Where(d => d.EmergencyReady && d.User.IsActive)
                .Select(d => new
                {
                    d.DoctorID,
                    d.User.FullName,
                    d.Specialty,
                    d.User.Phone
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(doctors));
        }
    }
}
