using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Medication
    {
        [Key]
        public int MedicationID { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; } // مسكنات، مضادات حيوية، إلخ

        [MaxLength(100)]
        public string? DosageForm { get; set; } // أقراص، شراب، حقن، إلخ

        [MaxLength(50)]
        public string? Unit { get; set; } // قرص، مل، أمبولة

        public int QuantityInStock { get; set; } = 0;

        public int MinStockLevel { get; set; } = 10; // الحد الأدنى للتنبيه

        public decimal PurchasePrice { get; set; } = 0; // سعر الشراء

        public decimal SellingPrice { get; set; } = 0; // سعر البيع

        [MaxLength(200)]
        public string? Manufacturer { get; set; } // الشركة المصنعة

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<DispenseRecord> DispenseRecords { get; set; } = new List<DispenseRecord>();
    }
}
