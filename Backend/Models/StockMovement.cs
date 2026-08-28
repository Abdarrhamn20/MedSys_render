using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    // رأس سند المخزن (إدخال / إخراج / تحويل)
    public class StockMovement
    {
        [Key]
        public int MovementID { get; set; }

        [Required, MaxLength(30)]
        public string MovementNumber { get; set; } = string.Empty;

        // In (إدخال) / Out (إخراج) / Transfer (تحويل)
        [Required, MaxLength(20)]
        public string MovementType { get; set; } = "In";

        public DateTime MovementDate { get; set; } = DateTime.Now;

        public int WarehouseID { get; set; }

        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; } = null!;

        // مخزن التحويل إليها (لنوع التحويل فقط)
        public int? ToWarehouseID { get; set; }

        [ForeignKey("ToWarehouseID")]
        public Warehouse? ToWarehouse { get; set; }

        [MaxLength(100)]
        public string ReferenceType { get; set; } = "Adjustment"; // Purchase, Adjustment, Damage, Transfer, ...

        public int? ReferenceID { get; set; }

        [MaxLength(300)]
        public string Notes { get; set; } = string.Empty;

        // Draft, Posted, Reversed
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Draft";

        public int CreatedByUserID { get; set; }

        [ForeignKey("CreatedByUserID")]
        public User? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? PostedByUserID { get; set; }

        [ForeignKey("PostedByUserID")]
        public User? PostedByUser { get; set; }

        public DateTime? PostedAt { get; set; }

        public ICollection<StockMovementItem> Items { get; set; } = new List<StockMovementItem>();
    }
}
