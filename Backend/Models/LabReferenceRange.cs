using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class LabReferenceRange
    {
        [Key]
        public int RangeID { get; set; }

        public int LabTestID { get; set; }

        [ForeignKey("LabTestID")]
        public virtual LabTest? LabTest { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; } = "All"; // Both, Male, Female

        public int MinAge { get; set; } = 0;
        public int MaxAge { get; set; } = 120;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NormalMin { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NormalMax { get; set; }

        [MaxLength(50)]
        public string? RangeNotes { get; set; }
    }
}
