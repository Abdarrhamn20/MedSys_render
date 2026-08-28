using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionID { get; set; }

        [ForeignKey("MedicalRecord")]
        public int RecordID { get; set; }

        [ForeignKey("Medication")]
        public int? MedicationID { get; set; } // ربط اختياري بمخزون الأدوية

        [Required, MaxLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Duration { get; set; }

        [MaxLength(300)]
        public string? Instructions { get; set; }

        [MaxLength(50)]
        public string? Frequency { get; set; }

        public int Quantity { get; set; } = 1; // الكمية المطلوبة

        [MaxLength(30)]
        public string DispenseStatus { get; set; } = "Pending"; // Pending, Dispensed, PartiallyDispensed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public MedicalRecord MedicalRecord { get; set; } = null!;
        public Medication? Medication { get; set; }
        public ICollection<DispenseRecord> DispenseRecords { get; set; } = new List<DispenseRecord>();
    }
}
