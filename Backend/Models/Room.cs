using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }

        public int WardID { get; set; }
        public Ward Ward { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string RoomNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string RoomType { get; set; } = "General"; // VIP, Private, General, ICU, Observation

        public decimal DailyRate { get; set; } = 0;

        public int MaxBeds { get; set; } = 2;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Bed> Beds { get; set; } = new List<Bed>();
    }
}
