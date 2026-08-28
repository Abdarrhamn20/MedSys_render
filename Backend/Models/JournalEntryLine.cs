using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class JournalEntryLine
    {
        [Key]
        public int JournalEntryLineID { get; set; }

        public int JournalEntryID { get; set; }

        [ForeignKey("JournalEntryID")]
        public JournalEntry JournalEntry { get; set; } = null!;

        public int AccountID { get; set; }

        [ForeignKey("AccountID")]
        public ChartAccount Account { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0.00m;

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
