using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class LabTest
    {
        [Key]
        public int LabTestID { get; set; }

        [Required, MaxLength(150)]
        public string TestName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty; // CBC, LFT, KFT, HBA1C

        [MaxLength(100)]
        public string Category { get; set; } = "General"; // Hematology, Biochemistry, Immunology, Microbiology

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 25.00m;

        [MaxLength(50)]
        public string Unit { get; set; } = "mg/dL";

        // === Advanced Lab: Panels + Device ===
        public bool IsPanel { get; set; } = false;

        public int? PanelID { get; set; }

        public int? DeviceID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual LabTest? ParentPanel { get; set; }
        public virtual ICollection<LabTest> PanelChildren { get; set; } = new List<LabTest>();
        public virtual LabDevice? Device { get; set; }
        public virtual ICollection<LabReferenceRange> ReferenceRanges { get; set; } = new List<LabReferenceRange>();
    }
}
