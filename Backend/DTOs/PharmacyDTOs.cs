namespace MedicalSystem.DTOs
{
    public class MedicationDTO
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? DosageForm { get; set; }
        public string? Unit { get; set; }
        public int QuantityInStock { get; set; }
        public int MinStockLevel { get; set; } = 10;
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class UpdateMedicationDTO
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Category { get; set; }
        public string? DosageForm { get; set; }
        public string? Unit { get; set; }
        public int? QuantityInStock { get; set; }
        public int? MinStockLevel { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class DispenseDTO
    {
        public int PrescriptionID { get; set; }
        public int? MedicationID { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }
    }

    public class MedicationRequestDTO
    {
        public string MedicationName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
