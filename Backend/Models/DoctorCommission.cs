using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class DoctorCommission
    {
        [Key]
        public int CommissionID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        public virtual User? Doctor { get; set; }

        [MaxLength(100)]
        public string? Specialty { get; set; }

        public int? ServiceID { get; set; }

        [Required]
        [MaxLength(20)]
        public string CommissionType { get; set; } = "Percentage"; // "Percentage" or "FixedAmount"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; } = 50.00m; // Example: 50% or 50 LYD

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
