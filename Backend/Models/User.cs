using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Role { get; set; } = "Patient"; // Admin, Doctor, Patient, Pharmacist, LabTechnician, Radiologist, Receptionist, Cashier, WarehouseKeeper, Accountant

        [MaxLength(20)]
        public string? Phone { get; set; }

        // الخزينة المخصصة لدور الكاشير (خزينته فقط)
        public int? AssignedTreasuryID { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public DoctorProfile? DoctorProfile { get; set; }
        public PatientProfile? PatientProfile { get; set; }
        public Treasury? AssignedTreasury { get; set; }
    }
}
