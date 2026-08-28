using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    // اشتراك جهاز/متصفح واحد لاستقبال Push Notifications
    public class WebPushSubscription
    {
        [Key]
        public int SubscriptionID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, MaxLength(500)]
        public string Endpoint { get; set; } = string.Empty;

        [Required, MaxLength(256)]
        public string P256DH { get; set; } = string.Empty;

        [Required, MaxLength(128)]
        public string Auth { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? UserAgent { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastUsedAt { get; set; }

        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}
