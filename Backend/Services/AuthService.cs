using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;
using MedicalSystem.Models;

namespace MedicalSystem.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return ApiResponse<AuthResponseDTO>.Fail("البريد الإلكتروني أو كلمة المرور غير صحيحة");

            var token = JwtHelper.GenerateToken(user.UserID, user.Email, user.Role, user.FullName, _config);

            int? profileId = null;
            string? specialty = null;
            if (user.Role == "Doctor")
            {
                var docProfile = await _context.DoctorProfiles
                    .Where(d => d.UserID == user.UserID)
                    .Select(d => new { d.DoctorID, d.Specialty })
                    .FirstOrDefaultAsync();
                if (docProfile != null)
                {
                    profileId = docProfile.DoctorID;
                    specialty = docProfile.Specialty;
                }
            }
            else if (user.Role == "Patient")
            {
                profileId = await _context.PatientProfiles
                    .Where(p => p.UserID == user.UserID)
                    .Select(p => p.PatientID)
                    .FirstOrDefaultAsync();
            }

            var response = new AuthResponseDTO
            {
                Token = token,
                User = new UserInfoDTO
                {
                    UserID = user.UserID,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    ProfileID = profileId,
                    AssignedTreasuryID = user.AssignedTreasuryID,
                    IsActive = user.IsActive,
                    Specialty = specialty
                }
            };

            return ApiResponse<AuthResponseDTO>.Ok(response, "تم تسجيل الدخول بنجاح");
        }

        public async Task<ApiResponse<AuthResponseDTO>> RegisterAsync(RegisterDTO dto)
        {
            // قائمة بيضاء بالأدوار المسموح بإنشائها ذاتياً — لا يُسمح بإنشاء أدوار إدارية أو صيدلانية عبر التسجيل الذاتي
            var allowedRoles = new[] { "Patient", "Doctor" };
            var requestedRole = string.IsNullOrWhiteSpace(dto.Role) ? "Patient" : dto.Role.Trim();
            if (!allowedRoles.Contains(requestedRole))
                return ApiResponse<AuthResponseDTO>.Fail("لا يُسمح بإنشاء هذا الدور عبر التسجيل الذاتي. يرجى التواصل مع مدير النظام.");

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return ApiResponse<AuthResponseDTO>.Fail("البريد الإلكتروني مسجل مسبقاً");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = requestedRole,
                Phone = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create profile based on role
            if (requestedRole == "Doctor")
            {
                var doctorProfile = new DoctorProfile
                {
                    UserID = user.UserID,
                    Specialty = dto.Specialty ?? "عام",
                    LicenseNumber = dto.LicenseNumber,
                    ConsultationDurationMinutes = 30
                };
                _context.DoctorProfiles.Add(doctorProfile);
                await _context.SaveChangesAsync();
            }
            else if (requestedRole == "Patient")
            {
                var patientProfile = new PatientProfile
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
                };
                _context.PatientProfiles.Add(patientProfile);
                await _context.SaveChangesAsync();
            }

            var token = JwtHelper.GenerateToken(user.UserID, user.Email, user.Role, user.FullName, _config);

            var response = new AuthResponseDTO
            {
                Token = token,
                User = new UserInfoDTO
                {
                    UserID = user.UserID,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    AssignedTreasuryID = user.AssignedTreasuryID,
                    IsActive = user.IsActive
                }
            };

            return ApiResponse<AuthResponseDTO>.Ok(response, "تم إنشاء الحساب بنجاح");
        }

        public async Task<ApiResponse<UserInfoDTO>> GetProfileAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserID == userId && u.IsActive)
                .Select(u => new UserInfoDTO
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    AssignedTreasuryID = u.AssignedTreasuryID,
                    IsActive = u.IsActive,
                    ProfileID = u.Role == "Doctor"
                        ? u.DoctorProfile!.DoctorID
                        : u.Role == "Patient"
                            ? u.PatientProfile!.PatientID
                            : (int?)null,
                    Specialty = u.Role == "Doctor"
                        ? u.DoctorProfile!.Specialty
                        : null
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return ApiResponse<UserInfoDTO>.Fail("المستخدم غير موجود");

            return ApiResponse<UserInfoDTO>.Ok(user);
        }

        public async Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return ApiResponse.Fail("المستخدم غير موجود");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
                return ApiResponse.Fail("كلمة المرور الحالية غير صحيحة");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return ApiResponse.Ok("تم تغيير كلمة المرور بنجاح");
        }

        public async Task<ApiResponse<object>> CheckClaimAccountAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return ApiResponse<object>.Fail("يرجى إدخال رقم الهاتف");

            var cleanedPhone = phone.Trim();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Phone == cleanedPhone && u.Role == "Patient");

            if (user == null)
            {
                return ApiResponse<object>.Fail("لم نجد حساباً مسجلاً لدى العيادة بهذا الرقم. يمكنك إنشاء حساب جديد.");
            }

            return ApiResponse<object>.Ok(new
            {
                isFound = true,
                fullName = user.FullName,
                email = user.Email,
                phone = user.Phone
            }, $"أهلاً بك يا {user.FullName}! وجدنا سجلك الطبي المسجل بالعيادة.");
        }

        public async Task<ApiResponse<AuthResponseDTO>> ClaimAccountAsync(ClaimAccountDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Phone) || string.IsNullOrWhiteSpace(dto.Password))
                return ApiResponse<AuthResponseDTO>.Fail("يرجى إدخال رقم الهاتف وكلمة المرور الجديدة");

            var cleanedPhone = dto.Phone.Trim();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Phone == cleanedPhone && u.Role == "Patient");

            if (user == null)
                return ApiResponse<AuthResponseDTO>.Fail("لم نجد حساباً مسجلاً لدى العيادة بهذا الرقم.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains("@clinic.com"))
            {
                user.Email = dto.Email.Trim();
            }

            await _context.SaveChangesAsync();

            var token = JwtHelper.GenerateToken(user.UserID, user.Email, user.Role, user.FullName, _config);
            var pId = await _context.PatientProfiles
                .Where(p => p.UserID == user.UserID)
                .Select(p => p.PatientID)
                .FirstOrDefaultAsync();

            var response = new AuthResponseDTO
            {
                Token = token,
                User = new UserInfoDTO
                {
                    UserID = user.UserID,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    ProfileID = pId,
                    AssignedTreasuryID = user.AssignedTreasuryID,
                    IsActive = user.IsActive
                }
            };

            return ApiResponse<AuthResponseDTO>.Ok(response, $"تم تفعيل وتأمين حسابك بنجاح يا {user.FullName}!");
        }
    }
}
