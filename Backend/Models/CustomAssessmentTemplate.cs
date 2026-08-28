using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class CustomAssessmentTemplate
    {
        [Key]
        public int TemplateID { get; set; }

        [ForeignKey("Doctor")]
        public int? DoctorID { get; set; } // في حال كان مخصصاً لطبيب معين، وإلا فهو عام للعيادة

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty; // عنوان الاستبيان

        [MaxLength(500)]
        public string? Description { get; set; } // وصف الاستبيان للمريض

        [Required]
        public string SchemaJson { get; set; } = string.Empty; // هيكل الأسئلة والخيارات بتنسيق JSON

        /// <summary>
        /// نوع القالب: "Custom" (افتراضي) | "PHQ9" | "GAD7" | "Standard"
        /// </summary>
        [MaxLength(20)]
        public string TemplateType { get; set; } = "Custom";

        /// <summary>
        /// هل هذا القالب معياري عالمي (Seed) ولا يمكن حذفه
        /// </summary>
        public bool IsStandard { get; set; } = false;

        /// <summary>
        /// الحد الأقصى للنواتج (Scoring Max) - للاستبيانات المعيارية مثل PHQ-9 (27) و GAD-7 (21)
        /// </summary>
        public int? MaxScore { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public DoctorProfile? Doctor { get; set; }
        public ICollection<PatientAssessment> PatientAssessments { get; set; } = new List<PatientAssessment>();
    }
}
