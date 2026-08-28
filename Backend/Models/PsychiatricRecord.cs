using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class PsychiatricRecord
    {
        [Key, ForeignKey("MedicalRecord")]
        public int RecordID { get; set; }

        [MaxLength(1000)]
        public string? Appearance { get; set; } // المظهر العام والهيئة

        [MaxLength(1000)]
        public string? Behavior { get; set; } // السلوك والنشاط الحركي

        [MaxLength(1000)]
        public string? Speech { get; set; } // الكلام ونبرة الصوت

        [MaxLength(1000)]
        public string? MoodAndAffect { get; set; } // المزاج والوجدان

        [MaxLength(1000)]
        public string? ThoughtProcess { get; set; } // مجرى التفكير

        [MaxLength(1000)]
        public string? ThoughtContent { get; set; } // محتوى التفكير

        [MaxLength(1000)]
        public string? Perception { get; set; } // الإدراك الحسي (الهلاوس)

        [MaxLength(1000)]
        public string? Cognition { get; set; } // الإدراك المعرفي والتركيز

        [MaxLength(1000)]
        public string? InsightAndJudgment { get; set; } // الاستبصار وبصيرة المريض

        public bool IsSpeechToTextUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public MedicalRecord MedicalRecord { get; set; } = null!;
    }
}
