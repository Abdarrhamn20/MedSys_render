using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class Treasury
    {
        [Key]
        public int TreasuryID { get; set; }

        [Required, MaxLength(50)]
        public string TreasuryName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string TreasuryNameAr { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string TreasuryCode { get; set; } = string.Empty;

        // الحساب المحاسبي المرتبط بالخزينة (مثال: 1010 الصندوق)
        public int AccountID { get; set; }

        [ForeignKey("AccountID")]
        public ChartAccount Account { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
