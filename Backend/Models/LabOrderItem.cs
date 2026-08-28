using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class LabOrderItem
    {
        [Key]
        public int LabOrderItemID { get; set; }

        public int LabOrderID { get; set; }

        [ForeignKey("LabOrderID")]
        public virtual LabOrder? LabOrder { get; set; }

        public int LabTestID { get; set; }

        [ForeignKey("LabTestID")]
        public virtual LabTest? LabTest { get; set; }

        [MaxLength(500)]
        public string? ResultValue { get; set; }

        [MaxLength(20)]
        public string ResultStatus { get; set; } = "Pending"; // Pending, Normal, High, Low, Critical

        [MaxLength(500)]
        public string? TechnicianNotes { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
