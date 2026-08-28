using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class InventoryItem
    {
        [Key]
        public int ItemID { get; set; }

        [Required, MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ItemNameAr { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public InventoryCategory Category { get; set; } = null!;

        // اختياري: ربط الصنف بدواء الصيدلية إن وُجد
        public int? MedicationID { get; set; }

        [ForeignKey("MedicationID")]
        public Medication? Medication { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; } = "قطعة"; // قرص، عبوة، مل، ...

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; } = 0;

        public int ReorderLevel { get; set; } = 10;

        [MaxLength(200)]
        public string? Manufacturer { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
