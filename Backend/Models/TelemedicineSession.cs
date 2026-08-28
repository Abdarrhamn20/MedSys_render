using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class TelemedicineSession
    {
        [Key]
        public int SessionID { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentID { get; set; }

        [Required, MaxLength(36)]
        public string RoomCode { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Waiting"; // Waiting, Active, Ended

        public int CreatedByUserID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        [MaxLength(500)]
        public string? SessionNotes { get; set; }

        // Navigation Properties
        public Appointment Appointment { get; set; } = null!;
    }
}
