using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Models
{
    public class Attachment
    {
        [Key]
        public int AttachmentID { get; set; }

        [ForeignKey("MedicalRecord")]
        public int? RecordID { get; set; }

        [ForeignKey("Patient")]
        public int? PatientID { get; set; }

        [Required, MaxLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string FileURL { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public MedicalRecord? MedicalRecord { get; set; }
        public PatientProfile? Patient { get; set; }
    }
}
