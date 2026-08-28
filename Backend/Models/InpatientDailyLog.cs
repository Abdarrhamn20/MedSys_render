using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class InpatientDailyLog
    {
        [Key]
        public int LogID { get; set; }

        public int AdmissionID { get; set; }
        public Admission Admission { get; set; } = null!;

        public int LoggedByUserID { get; set; }
        public User LoggedByUser { get; set; } = null!;

        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string? Temperature { get; set; } // e.g. 37.2 °C

        [MaxLength(20)]
        public string? BloodPressure { get; set; } // e.g. 120/80

        [MaxLength(20)]
        public string? PulseRate { get; set; } // e.g. 75 bpm

        [MaxLength(20)]
        public string? OxygenLevel { get; set; } // e.g. 98%

        public string? DoctorNotes { get; set; }

        public string? NursingNotes { get; set; }
    }
}
