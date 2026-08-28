namespace MedicalSystem.DTOs
{
    // === المخازن ===
    public class WarehouseDTO
    {
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseNameAr { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // === فئات الأصناف (شجرة) ===
    public class InventoryCategoryDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryNameAr { get; set; } = string.Empty;
        public int? ParentCategoryID { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // === الأصناف ===
    public class InventoryItemDTO
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public int? MedicationID { get; set; }
        public string Unit { get; set; } = "قطعة";
        public decimal PurchasePrice { get; set; } = 0;
        public decimal SellingPrice { get; set; } = 0;
        public int ReorderLevel { get; set; } = 10;
        public string? Manufacturer { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // === سطر سند مخزني ===
    public class StockMovementItemDTO
    {
        public int ItemID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; } = 0;
        public string? Notes { get; set; }
    }

    // === رأس سند مخزني ===
    public class StockMovementDTO
    {
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; } = "In"; // In, Out, Transfer
        public int WarehouseID { get; set; }
        public int? ToWarehouseID { get; set; }
        public string ReferenceType { get; set; } = "Adjustment";
        public int? ReferenceID { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<StockMovementItemDTO> Items { get; set; } = new List<StockMovementItemDTO>();
    }

    // === سطر جرد دوري ===
    public class StockCountItemDTO
    {
        public int ItemID { get; set; }
        public decimal CountedQuantity { get; set; }
        public string? Notes { get; set; }
    }

    // === رأس سند الجرد الدوري ===
    public class StockCountDTO
    {
        public DateTime CountDate { get; set; }
        public int WarehouseID { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<StockCountItemDTO> Items { get; set; } = new List<StockCountItemDTO>();
    }
}
