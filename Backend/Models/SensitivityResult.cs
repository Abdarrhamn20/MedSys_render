using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class SensitivityResult
    {
        [Key]
        public int SensitivityResultID { get; set; }

        public int CultureSensitivityID { get; set; }

        [ForeignKey("CultureSensitivityID")]
        public virtual CultureSensitivity? CultureSensitivity { get; set; }

        [Required, MaxLength(100)]
        public string AntibioticName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Interpretation { get; set; } = "Sensitive"; // Sensitive, Intermediate, Resistant

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ZoneDiameter { get; set; }
    }
}
