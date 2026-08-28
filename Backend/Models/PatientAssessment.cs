using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class PatientAssessment
    {
        [Key]
        public int AssessmentID { get; set; }

        public int PatientUserID { get; set; } // المريض الذي يجب أن يملأ الاستبيان

        [ForeignKey("CustomAssessmentTemplate")]
        public int TemplateID { get; set; } // الاستبيان المختار

        [Required]
        public string AnswersJson { get; set; } = "{}"; // إجابات المريض بتنسيق JSON

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Pending"; // Pending, Completed

        // Navigation
        [ForeignKey("PatientUserID")]
        public User PatientUser { get; set; } = null!;
        public CustomAssessmentTemplate CustomAssessmentTemplate { get; set; } = null!;
    }
}
