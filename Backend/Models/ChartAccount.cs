using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class ChartAccount
    {
        [Key]
        public int AccountID { get; set; }

        [Required, MaxLength(20)]
        public string AccountCode { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string AccountNameAr { get; set; } = string.Empty;

        // Asset, Liability, Equity, Revenue, Expense
        [Required, MaxLength(20)]
        public string AccountType { get; set; } = "Asset";

        public int? ParentAccountID { get; set; }

        [ForeignKey("ParentAccountID")]
        public ChartAccount? ParentAccount { get; set; }

        public List<ChartAccount> Children { get; set; } = new();

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; } = 0.00m;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
