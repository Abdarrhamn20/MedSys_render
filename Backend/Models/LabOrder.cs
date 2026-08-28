using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class LabOrder
    {
        [Key]
        public int LabOrderID { get; set; }

        public int PatientUserID { get; set; }

        [ForeignKey("PatientUserID")]
        public virtual User? PatientUser { get; set; }

        public int DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        public virtual User? Doctor { get; set; }

        public int LabTestID { get; set; }

        [ForeignKey("LabTestID")]
        public virtual LabTest? LabTest { get; set; }

        [MaxLength(50)]
        public string? ResultValue { get; set; }

        [MaxLength(20)]
        public string ResultStatus { get; set; } = "Pending"; // Pending, Normal, High, Low, Critical

        [MaxLength(30)]
        public string Status { get; set; } = "Requested"; // Requested, InProgress, Completed, Cancelled

        [MaxLength(500)]
        public string? ResultNotes { get; set; }

        [MaxLength(500)]
        public string? TechnicianNotes { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        [MaxLength(200)]
        public string? VerificationQRCode { get; set; }

        // Advanced Lab: multiple tests per order
        public virtual ICollection<LabOrderItem> Items { get; set; } = new List<LabOrderItem>();
    }
}
