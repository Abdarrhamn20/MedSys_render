using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class RadiologyOrder
    {
        [Key]
        public int RadiologyOrderID { get; set; }

        public int PatientUserID { get; set; }

        [ForeignKey("PatientUserID")]
        public virtual User? PatientUser { get; set; }

        public int DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        public virtual User? Doctor { get; set; }

        [Required, MaxLength(50)]
        public string Modality { get; set; } = "X-Ray";

        [Required, MaxLength(100)]
        public string BodyPart { get; set; } = "Chest";

        [MaxLength(30)]
        public string Status { get; set; } = "Requested"; // Requested, InProgress, Completed, Cancelled

        public string? ReportText { get; set; }

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public int? RadiologistID { get; set; }

        [ForeignKey("RadiologistID")]
        public virtual User? Radiologist { get; set; }
    }
}
