using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class InpatientCareExecution
    {
        [Key]
        public int ExecutionID { get; set; }

        public int OrderID { get; set; }
        public InpatientCareOrder Order { get; set; } = null!;

        public int ExecutedByUserID { get; set; }
        public User ExecutedByUser { get; set; } = null!;

        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Executed"; // Executed, Skipped, Refused

        public string? Notes { get; set; }

        // Optional vital signs captured during execution
        [MaxLength(20)]
        public string? VitalTemperature { get; set; }

        [MaxLength(20)]
        public string? VitalBloodPressure { get; set; }

        [MaxLength(20)]
        public string? VitalPulse { get; set; }

        [MaxLength(20)]
        public string? VitalOxygen { get; set; }
    }
}
