using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Admission
    {
        [Key]
        public int AdmissionID { get; set; }

        public int PatientID { get; set; }
        public PatientProfile Patient { get; set; } = null!;

        public int DoctorID { get; set; }
        public DoctorProfile Doctor { get; set; } = null!;

        public int BedID { get; set; }
        public Bed Bed { get; set; } = null!;

        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        public DateTime? DischargeDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string AdmissionReason { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Discharged, Transferred

        public string? DischargeSummary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // Navigation
        public ICollection<InpatientDailyLog> DailyLogs { get; set; } = new List<InpatientDailyLog>();
    }
}
