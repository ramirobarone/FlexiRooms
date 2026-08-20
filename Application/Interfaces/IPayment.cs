using Application.Models.Booking;

namespace Application.Interfaces
{
    public interface IPayment
    {
        Task CreatePayment(CheckoutBooking checkout);
    }
}