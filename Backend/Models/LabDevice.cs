using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class LabDevice
    {
        [Key]
        public int LabDeviceID { get; set; }

        [Required, MaxLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string DeviceCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DeviceModel { get; set; }

        [MaxLength(30)]
        public string ConnectionType { get; set; } = "Manual"; // Manual, HL7, ASTM, Serial

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
