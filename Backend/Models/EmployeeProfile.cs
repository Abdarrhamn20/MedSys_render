using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class EmployeeProfile
    {
        [Key]
        public int EmployeeID { get; set; }

        // قد يكون الموظف مرتبطاً بحساب دخول (طبيب/موظف استقبال/...) أو موظفاً غير داخل النظام
        public int? UserID { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        [Required, MaxLength(30)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        public DateTime HireDate { get; set; } = DateTime.Today;

        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(30)]
        public string? NationalID { get; set; }

        // FixedSalary (راتب شهري) | Commission (عمولات فقط) | Mixed (راتب + عمولات)
        [Required, MaxLength(20)]
        public string CompensationModel { get; set; } = "FixedSalary";

        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; } = 0m;

        [MaxLength(50)]
        public string? BankAccount { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<EmployeeCourse> Courses { get; set; } = new List<EmployeeCourse>();
        public ICollection<EmployeeLeave> Leaves { get; set; } = new List<EmployeeLeave>();
        public ICollection<SalaryRecord> SalaryRecords { get; set; } = new List<SalaryRecord>();
    }
}
