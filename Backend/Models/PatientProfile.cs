using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class PatientProfile
    {
        [Key]
        public int PatientID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        // === التركيبة الاسمية الليبية ===
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? FatherName { get; set; }

        [MaxLength(100)]
        public string? GrandfatherName { get; set; }

        [MaxLength(100)]
        public string? FamilyName { get; set; }

        // رقم الملف التسلسلي (PT-YYYY-NNNN)
        [MaxLength(20)]
        public string? FileNumber { get; set; }

        // الملف المندمج فيه (مسار تاريخي للدمج)
        public int? MergedIntoPatientID { get; set; }

        public DateTime? MergedAt { get; set; }

        [MaxLength(5)]
        public string? BloodType { get; set; }

        [MaxLength(500)]
        public string? ChronicDiseases { get; set; }

        [MaxLength(500)]
        public string? Allergies { get; set; }

        [MaxLength(500)]
        public string? GeneralNotes { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; } // ذكر, أنثى

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }

        // === Psychiatric Risk Level ===
        /// <summary>
        /// مستوى الخطورة السريرية للمريض النفسي:
        /// "Stable" = مستقر 🟢 | "Monitoring" = تحت الملاحظة 🟡 | "Critical" = حرج 🔴
        /// </summary>
        [MaxLength(20)]
        public string? RiskLevel { get; set; }

        public DateTime? RiskLevelUpdatedAt { get; set; }

        public int? RiskLevelUpdatedByUserID { get; set; }

        [MaxLength(500)]
        public string? RiskLevelNotes { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
