using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    // رأس سند الجرد الدوري
    public class StockCount
    {
        [Key]
        public int StockCountID { get; set; }

        [Required, MaxLength(30)]
        public string StockCountNumber { get; set; } = string.Empty;

        public DateTime CountDate { get; set; } = DateTime.Now;

        public int WarehouseID { get; set; }

        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; } = null!;

        // Draft, Posted, Reversed
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Draft";

        [MaxLength(300)]
        public string Notes { get; set; } = string.Empty;

        public int CreatedByUserID { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? PostedByUserID { get; set; }

        [ForeignKey("PostedByUserID")]
        public User? PostedByUser { get; set; }

        public DateTime? PostedAt { get; set; }

        public int? ReversedByUserID { get; set; }

        [ForeignKey("ReversedByUserID")]
        public User? ReversedByUser { get; set; }

        public DateTime? ReversedAt { get; set; }

        public ICollection<StockCountItem> Items { get; set; } = new List<StockCountItem>();
    }

    // سطر في سند الجرد: الكمية النظامية قبل الجرد والكمية الفعلية والفرق
    public class StockCountItem
    {
        [Key]
        public int StockCountItemID { get; set; }

        public int StockCountID { get; set; }

        [ForeignKey("StockCountID")]
        public StockCount StockCount { get; set; } = null!;

        public int ItemID { get; set; }

        [ForeignKey("ItemID")]
        public InventoryItem Item { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SystemQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CountedQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Difference => CountedQuantity - SystemQuantity;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } = 0;

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
