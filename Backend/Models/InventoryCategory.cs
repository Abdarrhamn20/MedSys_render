using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class InventoryCategory
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string CategoryNameAr { get; set; } = string.Empty;

        // الأب لتشكيل شجرة أصناف (يُترك null للفروع الرئيسية)
        public int? ParentCategoryID { get; set; }

        [ForeignKey("ParentCategoryID")]
        public InventoryCategory? ParentCategory { get; set; }

        public ICollection<InventoryCategory> Children { get; set; } = new List<InventoryCategory>();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
