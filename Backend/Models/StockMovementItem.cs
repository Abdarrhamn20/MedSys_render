using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    // سطر في سند المخزن: صنف + كمية
    public class StockMovementItem
    {
        [Key]
        public int StockMovementItemID { get; set; }

        public int MovementID { get; set; }

        [ForeignKey("MovementID")]
        public StockMovement Movement { get; set; } = null!;

        public int ItemID { get; set; }

        [ForeignKey("ItemID")]
        public InventoryItem Item { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice => Quantity * UnitPrice;

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
