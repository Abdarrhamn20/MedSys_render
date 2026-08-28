using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class RadiologyTemplate
    {
        [Key]
        public int TemplateID { get; set; }

        [Required, MaxLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Modality { get; set; } = "X-Ray";

        [MaxLength(100)]
        public string BodyPart { get; set; } = "Chest";

        [Required]
        public string DefaultReportText { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
