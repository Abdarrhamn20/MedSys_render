using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class SystemSetting
    {
        [Key]
        [MaxLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string SettingValue { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
