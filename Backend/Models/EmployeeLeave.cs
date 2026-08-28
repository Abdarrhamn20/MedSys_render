using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class EmployeeLeave
    {
        [Key]
        public int LeaveID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public EmployeeProfile Employee { get; set; } = null!;

        // Annual, Sick, Unpaid, Other
        [Required, MaxLength(20)]
        public string LeaveType { get; set; } = "Annual";

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Days { get; set; }

        [MaxLength(300)]
        public string? Reason { get; set; }

        // Pending, Approved, Rejected
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public int? ApprovedByUserID { get; set; }

        [ForeignKey("ApprovedByUserID")]
        public User? ApprovedByUser { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
