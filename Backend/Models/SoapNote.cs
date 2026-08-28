using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class SoapNote
    {
        [Key]
        public int SoapNoteID { get; set; }

        [ForeignKey("MedicalRecord")]
        public int RecordID { get; set; }

        public string? Subjective { get; set; }

        public string? Objective { get; set; }

        public string? Assessment { get; set; }

        public string? Plan { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public MedicalRecord MedicalRecord { get; set; } = null!;
    }
}
