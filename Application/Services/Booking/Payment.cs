using Application.Interfaces;
using Application.Models.Booking;
using Application.Models.CheckOut;
using Infrastructure.Models;
using Infrastructure.Repository;
using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Error;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Booking
{
    public sealed class Paymentt(
        PaymentClient paymentClient,
        IRepository<Room> roomRepository,
        IServiceGeneric<Bookings> bookingService,
        IRepository<PaymentTransaction> paymentRepository,
        UserManager<ApplicationUser> userManager) : IPayment
    {
        public async Task<ResponsePayment> CreatePayment(CheckoutBooking checkout)
        {
            ApplicationUser user = await userManager.Users.SingleOrDefaultAsync(user => user.UserGuid == checkout.RoomDto.UserGuid)
                ?? throw new InvalidOperationException("No se encontró el usuario de la transacción.");
            Room room = await roomRepository.GetByIdAsync(room => room.Id == checkout.RoomDto.IdRoom,
                query => query.Include(room => room.Cost));
            decimal amount = room.Cost?.CostPerTime
                ?? throw new InvalidOperationException("La habitación no tiene un precio configurado.");

            var request = new PaymentCreateRequest
            {
                TransactionAmount = amount,
                Token = checkout.Payment.Token,
                PaymentMethodId = checkout.Payment.PaymentMethodId,
                Installments = checkout.Payment.Installments,
                Description = $"Reserva de habitación {room.Id}",
                ExternalReference = $"room-{room.Id}-booking-{checkout.RoomDto.Date:yyyyMMdd}-{checkout.RoomDto.CheckInTimeId}",
                Payer = new PaymentPayerRequest
                {
                    Email = checkout.Payment.PayerEmail,
                    Identification = string.IsNullOrWhiteSpace(checkout.Payment.IdentificationNumber) ? null : new IdentificationRequest
                    {
                        Type = checkout.Payment.IdentificationType,
                        Number = checkout.Payment.IdentificationNumber
                    }
                }
            };

            string idempotencyKey = Guid.NewGuid().ToString();
            var requestOptions = new RequestOptions();
            requestOptions.CustomHeaders.Add("X-Idempotency-Key", idempotencyKey);

            Payment sdkResult;
            try
            {
                sdkResult = await paymentClient.CreateAsync(request, requestOptions);
            }
            catch (MercadoPagoApiException exception)
            {
                throw new InvalidOperationException("Mercado Pago rechazó la operación.", exception);
            }
            catch (MercadoPagoException exception)
            {
                throw new InvalidOperationException("No se pudo procesar el pago con Mercado Pago.", exception);
            }

            ResponsePayment result = new()
            {
                id = (int)(sdkResult.Id ?? 0),
                status = sdkResult.Status,
                status_detail = sdkResult.StatusDetail,
                currency_id = sdkResult.CurrencyId,
                payment_method_id = sdkResult.PaymentMethodId,
                installments = sdkResult.Installments ?? 0
            };

            Bookings? booking = result.status == "approved"
                ? await bookingService.Create(checkout.RoomDto)
                : null;

            await paymentRepository.CreateAsync(new PaymentTransaction
            {
                MercadoPagoPaymentId = result.id.ToString(),
                IdempotencyKey = idempotencyKey,
                Status = result.status ?? "unknown",
                StatusDetail = result.status_detail,
                CurrencyId = result.currency_id ?? "ARS",
                PaymentMethodId = result.payment_method_id ?? checkout.Payment.PaymentMethodId,
                Installments = result.installments,
                Amount = amount,
                CreatedAtUtc = DateTime.UtcNow,
                BookingId = booking?.Id,
                ApplicationUserId = user.Id
            });

            if (result.status != "approved")
                throw new InvalidOperationException("El pago no fue aprobado.");

            return result;
        }
    }
}
