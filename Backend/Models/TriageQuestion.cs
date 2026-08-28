using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class TriageQuestion
    {
        [Key]
        public int QuestionID { get; set; }

        [Required, MaxLength(300)]
        public string QuestionText { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string QuestionTextAr { get; set; } = string.Empty;

        public int Weight { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; } = string.Empty; // General, Cardiac, Respiratory, Neurological, Pain

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; }
    }
}
