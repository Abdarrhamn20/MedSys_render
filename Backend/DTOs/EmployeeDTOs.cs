namespace MedicalSystem.DTOs
{
    // === بطاقة الموظف ===
    public class EmployeeDTO
    {
        // رابط حساب دخول اختياري (الموظف قد يكون داخل النظام أو خارجه)
        public int? UserID { get; set; }
        // إن أُنشئ حساب دخول جديد للموظف
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Today;
        public string? Gender { get; set; }
        public string? NationalID { get; set; }

        // FixedSalary (راتب شهري) | Commission (عمولات فقط) | Mixed (راتب + عمولات)
        public string CompensationModel { get; set; } = "FixedSalary";
        public decimal BaseSalary { get; set; } = 0m;
        public string? BankAccount { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
    }

    // === الدورات التدريبية ===
    public class EmployeeCourseDTO
    {
        public string CourseName { get; set; } = string.Empty;
        public string? Provider { get; set; }
        public DateTime CourseDate { get; set; } = DateTime.Today;
        public string? CertificateNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
    }

    // === الإجازات ===
    public class EmployeeLeaveDTO
    {
        // Annual, Sick, Unpaid, Other
        public string LeaveType { get; set; } = "Annual";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    public class LeaveStatusDTO
    {
        // Approved, Rejected
        public string Status { get; set; } = "Approved";
    }

    // === الرواتب ===
    public class PayrollRunDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class SalaryAdjustDTO
    {
        public decimal Bonus { get; set; } = 0m;
        public decimal Deduction { get; set; } = 0m;
    }
}
