namespace MedicalSystem.DTOs
{
    public class SubscribePushDTO
    {
        public string Endpoint { get; set; } = string.Empty;
        public string P256DH { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
    }

    public class UnsubscribePushDTO
    {
        public string Endpoint { get; set; } = string.Empty;
    }
}
