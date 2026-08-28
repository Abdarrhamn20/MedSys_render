using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class DispenseRecord
    {
        [Key]
        public int DispenseID { get; set; }

        [ForeignKey("Prescription")]
        public int PrescriptionID { get; set; }

        [ForeignKey("Medication")]
        public int? MedicationID { get; set; }

        public int QuantityDispensed { get; set; }

        public decimal TotalPrice { get; set; }

        public int DispensedByUserID { get; set; } // الصيدلاني الذي صرف

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Dispensed"; // Dispensed, Returned

        [MaxLength(300)]
        public string? Notes { get; set; }

        public DateTime DispensedAt { get; set; } = DateTime.Now;

        // Navigation
        public Prescription Prescription { get; set; } = null!;
        public Medication? Medication { get; set; }

        [ForeignKey("DispensedByUserID")]
        public User DispensedByUser { get; set; } = null!;
    }
}
