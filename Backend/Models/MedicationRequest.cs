using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class MedicationRequest
    {
        [Key]
        public int RequestID { get; set; }

        [Required, MaxLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        public int DoctorUserID { get; set; }

        [Required, MaxLength(200)]
        public string DoctorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsResolved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("DoctorUserID")]
        public User DoctorUser { get; set; } = null!;
    }
}
