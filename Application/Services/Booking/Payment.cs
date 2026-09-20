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
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Application.Services.Booking
{
    public sealed class Paymentt(
        PaymentClient paymentClient,
        IRepository<Room> roomRepository,
        IServiceGeneric<Bookings> bookingService,
        IRepository<PaymentTransaction> paymentRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<Paymentt> logger) : IPayment
    {
        public async Task<ResponsePayment> CreatePayment(CheckoutBooking checkout)
        {
            logger.LogInformation(
                "Starting Mercado Pago payment creation for userGuid {UserGuid}, roomId {RoomId}, costId {CostId}, amount request pending.",
                checkout.RoomDto.UserGuid,
                checkout.RoomDto.IdRoom,
                checkout.RoomDto.CostId);

            ApplicationUser user = await userManager.Users.SingleOrDefaultAsync(user => user.UserGuid == checkout.RoomDto.UserGuid)
                ?? throw new InvalidOperationException("No se encontró el usuario de la transacción.");

            logger.LogInformation("Found user {UserId} for Mercado Pago payment.", user.Id);

            Room room = await roomRepository.GetByIdAsync(room => room.Id == checkout.RoomDto.IdRoom,
                query => query.Include(room => room.Costs));

            logger.LogInformation("Loaded room {RoomId} with {CostCount} configured cost(s).", room.Id, room.Costs?.Count ?? 0);

            decimal amount = room.Costs?.FirstOrDefault(cost => cost.Id == checkout.RoomDto.CostId)?.CostPerTime
                ?? room.Costs?.FirstOrDefault()?.CostPerTime
                ?? throw new InvalidOperationException("La habitación no tiene un precio configurado.");

            logger.LogInformation("Calculated payment amount {Amount} for room {RoomId}.", amount, room.Id);

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
            logger.LogInformation("Using idempotency key {IdempotencyKey} for Mercado Pago request.", idempotencyKey);

            var requestOptions = new RequestOptions();
            requestOptions.CustomHeaders.Add("X-Idempotency-Key", idempotencyKey);

            Payment sdkResult;
            try
            {
                sdkResult = await paymentClient.CreateAsync(request, requestOptions);
                logger.LogInformation(
                    "Mercado Pago responded with payment id {PaymentId}, status {Status}, detail {StatusDetail}.",
                    sdkResult.Id,
                    sdkResult.Status,
                    sdkResult.StatusDetail);
            }
            catch (MercadoPagoApiException exception)
            {
                logger.LogError(exception,
                    "Mercado Pago API rejected payment for room {RoomId}, user {UserId}, idempotency {IdempotencyKey}.",
                    room.Id,
                    user.Id,
                    idempotencyKey);
                throw new InvalidOperationException("Mercado Pago rechazó la operación.", exception);
            }
            catch (MercadoPagoException exception)
            {
                logger.LogError(exception,
                    "Mercado Pago SDK failed for room {RoomId}, user {UserId}, idempotency {IdempotencyKey}.",
                    room.Id,
                    user.Id,
                    idempotencyKey);
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

            logger.LogInformation("Payment result mapped. PaymentId {PaymentId}, status {Status}.", result.id, result.status);

            bool isApproved = string.Equals(result.status, "approved", StringComparison.OrdinalIgnoreCase);
            bool isPending = string.Equals(result.status, "in_process", StringComparison.OrdinalIgnoreCase)
                             || string.Equals(result.status, "pending", StringComparison.OrdinalIgnoreCase);

            Bookings? booking = isApproved
                ? await bookingService.Create(checkout.RoomDto)
                : null;

            if (booking is not null)
            {
                logger.LogInformation("Booking {BookingId} created for approved payment {PaymentId}.", booking.Id, result.id);
            }
            else
            {
                logger.LogWarning(
                    "Booking was not created for payment {PaymentId}. Current status {Status}, detail {StatusDetail}.",
                    result.id,
                    result.status,
                    result.status_detail);
            }

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

            logger.LogInformation(
                "Stored payment transaction for payment {PaymentId} and booking {BookingId}.",
                result.id,
                booking?.Id);

            if (isApproved)
            {
                logger.LogInformation("Mercado Pago payment completed successfully for payment {PaymentId}.", result.id);
                return result;
            }

            if (isPending)
            {
                logger.LogInformation(
                    "Payment {PaymentId} is pending with status {Status} and detail {StatusDetail}. Final confirmation must be handled asynchronously.",
                    result.id,
                    result.status,
                    result.status_detail);
                return result;
            }

            throw new InvalidOperationException($"El pago no fue aprobado. Estado: {result.status}, detalle: {result.status_detail}");
        }
    }
}
