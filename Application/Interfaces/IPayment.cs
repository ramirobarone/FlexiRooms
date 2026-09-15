using Application.Models.Booking;
using Application.Models.CheckOut;

namespace Application.Interfaces
{
    public interface IPayment
    {
        Task<ResponsePayment> CreatePayment(CheckoutBooking checkout);
    }
}