namespace MedicalSystem.DTOs
{
    // === الخزائن ===
    public class TreasuryDTO
    {
        public string TreasuryName { get; set; } = string.Empty;
        public string TreasuryNameAr { get; set; } = string.Empty;
        public string TreasuryCode { get; set; } = string.Empty;
        public int AccountID { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // === السندات ===
    public class VoucherDTO
    {
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; } = "Receipt"; // Receipt, Payment, Transfer
        public int TreasuryID { get; set; }
        public int? ToTreasuryID { get; set; }
        public int? AccountID { get; set; }
        public int? PatientUserID { get; set; }
        public int? InvoiceID { get; set; }
        public int? AppointmentID { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    // === الإقفال المالي ===
    public class FiscalClosureDTO
    {
        // تاريخ بصيغة yyyy-MM-dd أو null لفتح النظام
        public string? ClosedThrough { get; set; }
    }
}
