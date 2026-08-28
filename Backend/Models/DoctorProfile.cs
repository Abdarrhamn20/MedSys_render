using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class DoctorProfile
    {
        [Key]
        public int DoctorID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required, MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        public bool EmergencyReady { get; set; } = false;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? AvailableDays { get; set; } // e.g. "Sun,Mon,Tue,Wed,Thu"

        public TimeSpan? WorkStartTime { get; set; }
        public TimeSpan? WorkEndTime { get; set; }

        public int ConsultationDurationMinutes { get; set; } = 30;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; } = 100.00m;

        // Navigation Properties
        public User User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
