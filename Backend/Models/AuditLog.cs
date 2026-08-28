using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class AuditLog
    {
        [Key]
        public int LogID { get; set; }

        [Required, MaxLength(50)]
        public string ActionType { get; set; } = string.Empty; // e.g., "StatusChange", "PrescriptionAdded"

        [Required, MaxLength(100)]
        public string EntityType { get; set; } = string.Empty; // e.g., "Appointment", "MedicalRecord"

        public int EntityID { get; set; }

        public int UserID { get; set; } // The user who performed the action

        [Required, MaxLength(500)]
        public string Details { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}
