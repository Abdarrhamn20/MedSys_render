using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.DTOs
{
    public class SetCommissionDTO
    {
        [Required]
        public int DoctorID { get; set; }

        public string? Specialty { get; set; }

        public int? ServiceID { get; set; }

        [Required]
        public string CommissionType { get; set; } = "Percentage"; // Percentage, FixedAmount

        [Required]
        public decimal Value { get; set; } = 50.00m;
    }

    public class ExpressBookingDTO
    {
        [Required]
        public string PatientName { get; set; } = string.Empty;

        public string? PatientPhone { get; set; }

        public string? Gender { get; set; }

        [Required]
        public int DoctorID { get; set; }

        public string PaymentMethod { get; set; } = "Cash"; // Cash, POS, Card

        public decimal ConsultationFee { get; set; } = 50.00m;

        public string? Notes { get; set; }
    }

    public class DoctorLedgerSummaryDTO
    {
        public int DoctorID { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public decimal DoctorTotalEarnings { get; set; }
        public decimal ClinicTotalShare { get; set; }
        public int TotalConsultations { get; set; }
        public List<DoctorLedgerItemDTO> Transactions { get; set; } = new();
    }

    public class DoctorLedgerItemDTO
    {
        public int InvoiceID { get; set; }
        public int AppointmentID { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DoctorShare { get; set; }
        public decimal ClinicShare { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public DateTime Date { get; set; }
    }

    public class DailyCashReportSummaryDTO
    {
        public DateTime Date { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalPOS { get; set; }
        public decimal TotalOnline { get; set; }
        public decimal GrandTotal { get; set; }
        public int TotalInvoices { get; set; }
        public List<DailyCashItemDTO> Payments { get; set; } = new();
    }

    public class DailyCashItemDTO
    {
        public int InvoiceID { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime Time { get; set; }
    }
}
