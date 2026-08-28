using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class JournalEntry
    {
        [Key]
        public int JournalEntryID { get; set; }

        [Required, MaxLength(30)]
        public string EntryNumber { get; set; } = string.Empty;

        public DateTime EntryDate { get; set; } = DateTime.Now;

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        // Manual, Invoice, Payment, Salary, Commission, ... (نظام التوليد الآلي لاحقاً)
        [MaxLength(30)]
        public string? SourceModule { get; set; }

        public int? SourceReferenceID { get; set; }

        // Draft, Posted, Reversed
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Draft";

        public int CreatedByUserID { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PostedAt { get; set; }

        public int? PostedByUserID { get; set; }

        [ForeignKey("PostedByUserID")]
        public User? PostedByUser { get; set; }

        public List<JournalEntryLine> Lines { get; set; } = new();
    }
}
