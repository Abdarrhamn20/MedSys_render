using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }

        public int PatientUserID { get; set; }

        public int? AppointmentID { get; set; }

        public int? DispenseRecordID { get; set; }

        public int? LabOrderID { get; set; }

        public int? RadiologyOrderID { get; set; }

        [Required, MaxLength(50)]
        public string InvoiceType { get; set; } = "Consultation"; // Consultation, Pharmacy, Inpatient, Laboratory, Radiology

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Cancelled, Refunded

        [MaxLength(30)]
        public string? PaymentMethod { get; set; } // Cash, Card, Insurance

        [MaxLength(100)]
        public string? TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PaidAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DoctorShare { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ClinicShare { get; set; } = 0.00m;

        public int? DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        public User? Doctor { get; set; }

        public int? DoctorCommissionID { get; set; }

        [ForeignKey("DoctorCommissionID")]
        public DoctorCommission? DoctorCommission { get; set; }

        // Navigation
        [ForeignKey("PatientUserID")]
        public User PatientUser { get; set; } = null!;

        [ForeignKey("AppointmentID")]
        public Appointment? Appointment { get; set; }

        [ForeignKey("DispenseRecordID")]
        public DispenseRecord? DispenseRecord { get; set; }

        [ForeignKey("LabOrderID")]
        public LabOrder? LabOrder { get; set; }

        [ForeignKey("RadiologyOrderID")]
        public RadiologyOrder? RadiologyOrder { get; set; }
    }
}
