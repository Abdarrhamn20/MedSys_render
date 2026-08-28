using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class CultureSensitivity
    {
        [Key]
        public int CultureSensitivityID { get; set; }

        public int LabOrderItemID { get; set; }

        [ForeignKey("LabOrderItemID")]
        public virtual LabOrderItem? LabOrderItem { get; set; }

        [MaxLength(200)]
        public string? Organism { get; set; }

        [MaxLength(20)]
        public string? GramStain { get; set; } // Positive, Negative

        [MaxLength(20)]
        public string CultureStatus { get; set; } = "NoGrowth"; // NoGrowth, Growth

        [MaxLength(50)]
        public string? QuantitativeResult { get; set; } // خفيف، متوسط، كثيف

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<SensitivityResult> SensitivityResults { get; set; } = new List<SensitivityResult>();
    }
}
