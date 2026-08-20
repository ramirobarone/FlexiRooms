using Application.Interfaces;
using Application.Models.Booking;
using Application.Models.CheckOut;
using Infrastructure.ServiceHttp;

namespace Application.Services.Booking
{
    public sealed class Paymentt(IHttpClientService<RequestPayment, ResponsePayment> httpClientService) : IPayment
    {
        public async Task CreatePayment(CheckoutBooking checkout)
        {
            //RequestPayment requestPayment = new RequestPayment()
            //{
            //    transaction_amount = checkout.RoomDto.Price,
            //     = new Payer()
            //    {
            //        email = checkout.billingData.Email,
            //    },
            //};

            //var resultPayment = await httpClientService.Post("https://api.mercadopago.com/v1/payments", requestPayment);

            //if (resultPayment.status == "approved")
                //await bookings.Create(checkout);
            //else
                //throw new Exception("Problemas al realizar el pago.");
        }
    }
}
