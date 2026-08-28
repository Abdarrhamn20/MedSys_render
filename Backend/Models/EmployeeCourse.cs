using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class EmployeeCourse
    {
        [Key]
        public int CourseID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public EmployeeProfile Employee { get; set; } = null!;

        [Required, MaxLength(150)]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Provider { get; set; }

        public DateTime CourseDate { get; set; } = DateTime.Today;

        [MaxLength(50)]
        public string? CertificateNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
