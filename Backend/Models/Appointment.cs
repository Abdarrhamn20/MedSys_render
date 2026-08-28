using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppID { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }

        [ForeignKey("Priority")]
        public int PriorityID { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, InProgress, Completed, Cancelled

        public int TriageScore { get; set; } = 0;

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(20)]
        public string AppointmentType { get; set; } = "WalkIn"; // WalkIn, Online

        public int QueueNumber { get; set; } = 1;

        [MaxLength(30)]
        public string? PaymentMethod { get; set; } = "Cash"; // Cash, POS, Card

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!; // Optimistic Locking

        // Navigation Properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
        public Priority Priority { get; set; } = null!;
        public MedicalRecord? MedicalRecord { get; set; }
    }
}
