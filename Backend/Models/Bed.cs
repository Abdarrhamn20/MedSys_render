using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Bed
    {
        [Key]
        public int BedID { get; set; }

        public int RoomID { get; set; }
        public Room Room { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string BedNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Vacant"; // Vacant, Occupied, Maintenance, Reserved

        [MaxLength(255)]
        public string? Notes { get; set; }

        // Navigation
        public ICollection<Admission> Admissions { get; set; } = new List<Admission>();
    }
}
