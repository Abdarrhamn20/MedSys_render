using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.DTOs
{
    // ==========================================
    //  SOAP Notes DTOs
    // ==========================================

    public class SaveSoapNoteDTO
    {
        public string? Subjective { get; set; }  // S - شعور المريض وشكواه
        public string? Objective { get; set; }   // O - الملاحظات السريرية والفحص
        public string? Assessment { get; set; }  // A - التشخيص والتقييم
        public string? Plan { get; set; }        // P - خطة العلاج
    }

    // ==========================================
    //  Risk Level DTOs
    // ==========================================

    public class UpdateRiskLevelDTO
    {
        [Required, MaxLength(20)]
        public string RiskLevel { get; set; } = "Stable"; // Stable | Monitoring | Critical

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    // ==========================================
    //  MSE Quick-Pick Option DTO
    // ==========================================

    public class SavePsychiatricRecordDTO
    {
        [MaxLength(1000)]
        public string? Appearance { get; set; }

        [MaxLength(1000)]
        public string? Behavior { get; set; }

        [MaxLength(1000)]
        public string? Speech { get; set; }

        [MaxLength(1000)]
        public string? MoodAndAffect { get; set; }

        [MaxLength(1000)]
        public string? ThoughtProcess { get; set; }

        [MaxLength(1000)]
        public string? ThoughtContent { get; set; }

        [MaxLength(1000)]
        public string? Perception { get; set; }

        [MaxLength(1000)]
        public string? Cognition { get; set; }

        [MaxLength(1000)]
        public string? InsightAndJudgment { get; set; }

        public bool IsSpeechToTextUsed { get; set; }
    }

    // ==========================================
    //  Assessment Template DTOs
    // ==========================================

    public class CreateTemplateDTO
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public string SchemaJson { get; set; } = string.Empty; // JSON schema defining fields

        /// <summary>
        /// نوع القالب: "Custom" (افتراضي) | "PHQ9" | "GAD7" | "Standard"
        /// </summary>
        public string? TemplateType { get; set; } = "Custom";

        /// <summary>
        /// هل هذا القالب معياري عالمي (Seed) ولا يمكن حذفه
        /// </summary>
        public bool IsStandard { get; set; } = false;
    }

    public class AssignAssessmentDTO
    {
        public int PatientUserID { get; set; }
        public int TemplateID { get; set; }
    }

    public class SubmitAnswersDTO
    {
        [Required]
        public string AnswersJson { get; set; } = "{}";
    }
}
