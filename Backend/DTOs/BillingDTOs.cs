namespace MedicalSystem.DTOs
{
    public class CardPaymentDTO
    {
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;
    }
}
