using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    // إشعارات داخل النظام تظهر للمستخدم في حسابه (جرس التنبيهات)
    public class UserNotification
    {
        [Key]
        public int NotificationID { get; set; }

        // المستخدم المستلم للإشعار
        [Required]
        public int UserID { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Message { get; set; }

        // نوع الإشعار: AppointmentTimeReached, TelemedicineStarted, AppointmentConfirmed, AppointmentCancelled, LabResultReady, RadiologyResultReady, PrescriptionReady ...
        [MaxLength(50)]
        public string Type { get; set; } = "General";

        // الكيان المرتبط (Appointment, TelemedicineSession ...) ورقمه لفتح الصفحة المناسبة عند النقر
        [MaxLength(50)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityID { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}
