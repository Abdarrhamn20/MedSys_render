using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class Voucher
    {
        [Key]
        public int VoucherID { get; set; }

        [Required, MaxLength(30)]
        public string VoucherNumber { get; set; } = string.Empty;

        // Receipt (سند قبض), Payment (سند صرف), Transfer (سند تحويل)
        [Required, MaxLength(20)]
        public string VoucherType { get; set; } = "Receipt";

        public DateTime VoucherDate { get; set; } = DateTime.Now;

        public int TreasuryID { get; set; }

        [ForeignKey("TreasuryID")]
        public Treasury Treasury { get; set; } = null!;

        // خزينة التحويل إليها (لنوع التحويل فقط)
        public int? ToTreasuryID { get; set; }

        [ForeignKey("ToTreasuryID")]
        public Treasury? ToTreasury { get; set; }

        // الحساب المقابل (إلزامي للقبض/الصرف)
        public int? AccountID { get; set; }

        [ForeignKey("AccountID")]
        public ChartAccount? Account { get; set; }

        // اختياري: المريض المرتبط (مذنيه / تحصيل)
        public int? PatientUserID { get; set; }

        [ForeignKey("PatientUserID")]
        public User? PatientUser { get; set; }

        // اختياري: ربط السند بفاتورة محصّلة
        public int? InvoiceID { get; set; }

        [ForeignKey("InvoiceID")]
        public Invoice? Invoice { get; set; }

        // اختياري: ربط السند بحجز OPD
        public int? AppointmentID { get; set; }

        [ForeignKey("AppointmentID")]
        public Appointment? Appointment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

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
    }
}
