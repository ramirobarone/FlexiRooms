namespace Application.Models.Booking
{
    public class CheckoutBooking : BookingBase
    {

        public RoomBooking RoomDto { get; set; }
        public BillingData billingData { get; set; }
        public CreditCard creditCard { get; set; }
    }
}
