namespace MedicalSystem.DTOs
{
    // === Auth DTOs ===
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = "Patient";
        // Cashier / Treasury assignment
        public int? AssignedTreasuryID { get; set; }
        // Doctor fields
        public string? Specialty { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal ConsultationFee { get; set; } = 0;
        // Patient fields
        public string? BloodType { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        // التركيبة الاسمية الليبية
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandfatherName { get; set; }
        public string? FamilyName { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UserInfoDTO User { get; set; } = null!;
    }

    public class UserInfoDTO
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? ProfileID { get; set; }
        public int? AssignedTreasuryID { get; set; }
        public string? AssignedTreasuryNameAr { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Specialty { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? BloodType { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? RiskLevel { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class CheckClaimAccountDTO
    {
        public string Phone { get; set; } = string.Empty;
    }

    public class ClaimAccountDTO
    {
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    // === Generic API Response ===
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "تمت العملية بنجاح")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message = "حدث خطأ")
            => new() { Success = false, Message = message };
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ApiResponse Ok(string message = "تمت العملية بنجاح")
            => new() { Success = true, Message = message };

        public static ApiResponse Fail(string message = "حدث خطأ")
            => new() { Success = false, Message = message };
    }

    // === Pagination ===
    public class PaginatedResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "تمت العملية بنجاح";
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class FacilityModeDTO
    {
        public string FacilityMode { get; set; } = "General"; // General, Psychiatric, Hybrid
    }
}
