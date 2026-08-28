using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class SalaryRecord
    {
        [Key]
        public int SalaryRecordID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public EmployeeProfile Employee { get; set; } = null!;

        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionAmount { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonus { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deduction { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossSalary { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; } = 0m;

        // Draft, Posted, Reversed
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Draft";

        public int? JournalEntryID { get; set; }

        [ForeignKey("JournalEntryID")]
        public JournalEntry? JournalEntry { get; set; }

        public int CreatedByUserID { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PostedAt { get; set; }
    }
}
