using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class HealthService
    {
        [Key]
        public int ServiceID { get; set; }

        [Required, MaxLength(200)]
        public string ServiceName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ServiceNameAr { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; } // RoomService, Nursing, Diagnostic, Procedure, Diet

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        [MaxLength(50)]
        public string? Unit { get; set; } = "مرة"; // مرة، يوم، جلسة

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
