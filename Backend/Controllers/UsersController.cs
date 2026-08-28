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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users?search=&role=&page=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,LabTechnician,Radiologist,Pharmacist,Receptionist,Cashier")]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search) || (u.Phone != null && u.Phone.Contains(search)));

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserInfoDTO
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    AssignedTreasuryID = u.AssignedTreasuryID,
                    AssignedTreasuryNameAr = u.AssignedTreasury != null ? u.AssignedTreasury.TreasuryNameAr : null,
                    ProfileID = u.Role == "Doctor"
                        ? u.DoctorProfile!.DoctorID
                        : u.Role == "Patient"
                            ? u.PatientProfile!.PatientID
                            : (int?)null,
                    Specialty = u.DoctorProfile != null ? u.DoctorProfile.Specialty : null,
                    LicenseNumber = u.DoctorProfile != null ? u.DoctorProfile.LicenseNumber : null,
                    ConsultationFee = u.DoctorProfile != null ? u.DoctorProfile.ConsultationFee : 0,
                    BloodType = u.PatientProfile != null ? u.PatientProfile.BloodType : null,
                    Gender = u.PatientProfile != null ? u.PatientProfile.Gender : null,
                    DateOfBirth = u.PatientProfile != null ? u.PatientProfile.DateOfBirth : null,
                    RiskLevel = u.PatientProfile != null ? u.PatientProfile.RiskLevel : null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<UserInfoDTO>
            {
                Data = users,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserID == id)
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.Phone,
                    u.IsActive,
                    u.CreatedAt,
                    u.AssignedTreasuryID,
                    AssignedTreasuryNameAr = u.AssignedTreasury != null ? u.AssignedTreasury.TreasuryNameAr : null,
                    DoctorProfile = u.DoctorProfile != null ? new
                    {
                        u.DoctorProfile.DoctorID,
                        u.DoctorProfile.Specialty,
                        u.DoctorProfile.LicenseNumber,
                        u.DoctorProfile.EmergencyReady,
                        u.DoctorProfile.Bio,
                        u.DoctorProfile.ImageUrl,
                        u.DoctorProfile.ConsultationFee,
                        u.DoctorProfile.AvailableDays,
                        u.DoctorProfile.WorkStartTime,
                        u.DoctorProfile.WorkEndTime
                    } : null,
                    PatientProfile = u.PatientProfile != null ? new
                    {
                        u.PatientProfile.PatientID,
                        u.PatientProfile.BloodType,
                        u.PatientProfile.ChronicDiseases,
                        u.PatientProfile.Allergies,
                        u.PatientProfile.Gender,
                        u.PatientProfile.DateOfBirth
                    } : null
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(ApiResponse.Fail("المستخدم غير موجود"));

            var currentUserId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            // Admin يرى أي مستخدم؛ المستخدم يرى نفسه فقط؛ باقي الأدوار ممنوعة
            if (role != "Admin" && currentUserId != id)
                return Forbid();

            return Ok(ApiResponse<object>.Ok(user));
        }

        // POST: api/users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(ApiResponse.Fail("الاسم والبريد الإلكتروني وكلمة المرور مطلوبة."));

            var allowedRoles = new[] { "Admin", "Doctor", "Patient", "Pharmacist", "LabTechnician", "Radiologist", "Receptionist", "Cashier", "WarehouseKeeper", "Accountant" };
            if (string.IsNullOrWhiteSpace(dto.Role) || !allowedRoles.Contains(dto.Role))
                return BadRequest(ApiResponse.Fail("الدور المحدد غير صالح."));

            if (dto.Role == "Doctor" && dto.ConsultationFee < 0)
                return BadRequest(ApiResponse.Fail("رسوم الكشف لا يمكن أن تكون قيمة سالبة."));

            if (dto.Role == "Cashier")
            {
                if (!dto.AssignedTreasuryID.HasValue)
                    return BadRequest(ApiResponse.Fail("يجب تحديد الخزينة المخصصة للكاشير."));
                var treasury = await _context.Treasuries.FindAsync(dto.AssignedTreasuryID.Value);
                if (treasury == null || !treasury.IsActive)
                    return BadRequest(ApiResponse.Fail("الخزينة المخصصة للكاشير غير موجودة أو غير مفعّلة."));
            }

            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest(ApiResponse.Fail("البريد الإلكتروني مسجل مسبقاً"));

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                Phone = dto.Phone,
                AssignedTreasuryID = dto.Role == "Cashier" ? dto.AssignedTreasuryID : null,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (dto.Role == "Doctor")
            {
                _context.DoctorProfiles.Add(new DoctorProfile
                {
                    UserID = user.UserID,
                    Specialty = dto.Specialty ?? "عام",
                    LicenseNumber = dto.LicenseNumber,
                    ConsultationFee = dto.ConsultationFee
                });
                await _context.SaveChangesAsync();
            }
            else if (dto.Role == "Patient")
            {
                _context.PatientProfiles.Add(new PatientProfile
                {
                    UserID = user.UserID,
                    BloodType = dto.BloodType,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    FirstName = dto.FirstName,
                    FatherName = dto.FatherName,
                    GrandfatherName = dto.GrandfatherName,
                    FamilyName = dto.FamilyName,
                    FileNumber = await FileNumberHelper.GenerateNextAsync(_context)
                });
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.Ok("تم إنشاء المستخدم بنجاح"));
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RegisterDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(ApiResponse.Fail("المستخدم غير موجود"));

            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(ApiResponse.Fail("الاسم والبريد الإلكتروني مطلوبان."));

            var emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserID != id);
            if (emailTaken)
                return BadRequest(ApiResponse.Fail("البريد الإلكتروني مسجل لمستخدم آخر"));

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;

            // تحديث الخزينة المخصصة للكاشير إن أُرسلت
            if (user.Role == "Cashier")
            {
                if (!dto.AssignedTreasuryID.HasValue)
                    return BadRequest(ApiResponse.Fail("يجب تحديد الخزينة المخصصة للكاشير."));
                var treasury = await _context.Treasuries.FindAsync(dto.AssignedTreasuryID.Value);
                if (treasury == null || !treasury.IsActive)
                    return BadRequest(ApiResponse.Fail("الخزينة المخصصة للكاشير غير موجودة أو غير مفعّلة."));
                user.AssignedTreasuryID = dto.AssignedTreasuryID.Value;
            }

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم تحديث بيانات المستخدم بنجاح"));
        }

        // PUT: api/users/5/toggle-active
        [HttpPut("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(ApiResponse.Fail("المستخدم غير موجود"));

            var currentUserId = JwtHelper.GetUserIdFromClaims(User);
            if (user.UserID == currentUserId)
                return BadRequest(ApiResponse.Fail("لا يمكنك تعطيل حسابك الخاص."));

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            var msg = user.IsActive ? "تم تفعيل الحساب" : "تم تعطيل الحساب";
            return Ok(ApiResponse.Ok(msg));
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                .Include(u => u.DoctorProfile)
                .Include(u => u.PatientProfile)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound(ApiResponse.Fail("المستخدم غير موجود"));

            _context.Users.Remove(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(ApiResponse.Fail("لا يمكن حذف المستخدم لوجود سجلات مرتبطة به (مواعيد/فواتير/فحوصات). يُفضّل تعطيل الحساب بدلاً من الحذف."));
            }
            return Ok(ApiResponse.Ok("تم حذف المستخدم بنجاح"));
        }

        // GET: api/users/stats
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
                Doctors = await _context.Users.CountAsync(u => u.Role == "Doctor"),
                Patients = await _context.Users.CountAsync(u => u.Role == "Patient"),
                Admins = await _context.Users.CountAsync(u => u.Role == "Admin")
            };

            return Ok(ApiResponse<object>.Ok(stats));
        }
    }
}
