using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Priority
    {
        [Key]
        public int PriorityID { get; set; }

        [Required, MaxLength(30)]
        public string LevelName { get; set; } = string.Empty; // Normal, Urgent, Emergency

        [Required, MaxLength(30)]
        public string LevelNameAr { get; set; } = string.Empty; // عادي, عاجل, طوارئ

        public int Weight { get; set; }

        [MaxLength(10)]
        public string ColorCode { get; set; } = string.Empty; // #2DC653, #FF9F1C, #E63946

        [MaxLength(30)]
        public string Icon { get; set; } = string.Empty; // fa-check-circle, fa-exclamation-triangle, fa-ambulance

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
