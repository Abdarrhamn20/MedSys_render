using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class MedicalRecord
    {
        [Key]
        public int RecordID { get; set; }

        [ForeignKey("Appointment")]
        public int AppID { get; set; }

        [Required, MaxLength(1000)]
        public string Diagnosis { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? DiagnosisAr { get; set; }

        [MaxLength(2000)]
        public string? TreatmentPlan { get; set; }

        [MaxLength(2000)]
        public string? DoctorNotes { get; set; }

        [MaxLength(500)]
        public string? Symptoms { get; set; }

        [MaxLength(500)]
        public string? Recommendations { get; set; }

        public bool RequiresFollowUp { get; set; } = false;

        public DateTime? FollowUpDate { get; set; }

        [MaxLength(500)]
        public string? FollowUpNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Appointment Appointment { get; set; } = null!;
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public PsychiatricRecord? PsychiatricRecord { get; set; }
        public SoapNote? SoapNote { get; set; }
    }
}
