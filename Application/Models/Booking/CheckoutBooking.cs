namespace Application.Models.Booking
{
    public class CheckoutBooking : BookingBase
    {
        public required BookingInputDto RoomDto { get; set; }
        public required PaymentFormData Payment { get; set; }
    }

    public class PaymentFormData
    {
        public required string Token { get; set; }
        public required string PaymentMethodId { get; set; }
        public string? IssuerId { get; set; }
        public int Installments { get; set; }
        public required string PayerEmail { get; set; }
        public string? IdentificationType { get; set; }
        public string? IdentificationNumber { get; set; }
    }
}
