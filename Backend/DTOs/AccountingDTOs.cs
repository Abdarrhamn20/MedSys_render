namespace MedicalSystem.DTOs
{
    // === شجرة الحسابات ===
    public class ChartAccountDTO
    {
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string AccountType { get; set; } = "Asset";
        public int? ParentAccountID { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // === قيد محاسبي ===
    public class JournalEntryDTO
    {
        public DateTime EntryDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? SourceModule { get; set; }
        public int? SourceReferenceID { get; set; }
        public List<JournalEntryLineDTO> Lines { get; set; } = new();
    }

    public class JournalEntryLineDTO
    {
        public int AccountID { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Notes { get; set; }
    }
}
