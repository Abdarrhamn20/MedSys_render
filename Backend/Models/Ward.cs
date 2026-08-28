using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Ward
    {
        [Key]
        public int WardID { get; set; }

        [Required]
        [MaxLength(100)]
        public string WardName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string WardNameAr { get; set; } = string.Empty;

        [MaxLength(20)]
        public string GenderType { get; set; } = "Mixed"; // Male, Female, Mixed

        public int FloorNumber { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
