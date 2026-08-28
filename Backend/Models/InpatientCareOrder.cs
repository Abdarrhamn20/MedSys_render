using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class InpatientCareOrder
    {
        [Key]
        public int OrderID { get; set; }

        public int AdmissionID { get; set; }
        public Admission Admission { get; set; } = null!;

        public int? HealthServiceID { get; set; }

        [ForeignKey("HealthServiceID")]
        public HealthService? HealthService { get; set; }

        [Required]
        [MaxLength(30)]
        public string OrderType { get; set; } = "Medication"; // Medication, VitalCheck, NursingProcedure, Diet, IVFluid, HealthService

        [Required]
        [MaxLength(255)]
        public string OrderDescription { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Frequency { get; set; } = "Once"; // Once, Every4Hours, Every8Hours, Every12Hours, Daily

        public DateTime ScheduledTime { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } = 0.00m;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Executed, Cancelled, Overdue

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedByUserID { get; set; }
        public User CreatedByUser { get; set; } = null!;

        // Navigation
        public ICollection<InpatientCareExecution> Executions { get; set; } = new List<InpatientCareExecution>();
    }
}
