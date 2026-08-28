namespace MedicalSystem.DTOs
{
    public class NotificationDTO
    {
        public int NotificationID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Type { get; set; } = "General";
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityID { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
