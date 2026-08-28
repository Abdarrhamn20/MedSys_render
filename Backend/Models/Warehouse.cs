using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseID { get; set; }

        [Required, MaxLength(50)]
        public string WarehouseName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string WarehouseNameAr { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string WarehouseCode { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
