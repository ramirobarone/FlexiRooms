using System.Globalization;
using System.Text.Json;
using Application.Interfaces;
using Application.Models.CheckOut;
using Infrastructure.Context;
using Infrastructure.Models;
using MercadoPago.Client.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoPagoController(
        PaymentClient paymentClient,
        FlexiRoomsContext flexiRoomsContext,
        IServiceGeneric<Bookings> bookingService,
        IMercadoPagoAccountService mercadoPagoAccounts,
        UserManager<ApplicationUser> userManager,
        ILogger<MercadoPagoController> logger) : ControllerBase
    {
        [HttpGet("webhook")]
        [HttpPost("webhook")]
        public async Task<IActionResult> LegacyWebhook(CancellationToken cancellationToken)
        {
            Request.EnableBuffering();

            string body;
            using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync(cancellationToken);
                Request.Body.Position = 0;
            }

            logger.LogInformation(
                "MercadoPago legacy webhook recibido. Method: {Method}, Path: {Path}, Query: {Query}, Headers: {@Headers}, Body: {Body}",
                Request.Method,
                Request.Path,
                Request.QueryString.Value,
                Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString()),
                body);

            WebHookPaymentCreatedDto? jsonPayment = null;
            try
            {
                jsonPayment = System.Text.Json.JsonSerializer.Deserialize<WebHookPaymentCreatedDto>(
                    body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (System.Text.Json.JsonException exception)
            {
                logger.LogWarning(exception, "MercadoPago webhook con cuerpo inválido.");
            }

            string? paymentIdText = jsonPayment?.Data?.Id ?? TryGetPaymentId(Request, body)?.ToString(CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(paymentIdText) || paymentIdText == "0" || !long.TryParse(paymentIdText, out long paymentId))
            {
                logger.LogWarning("MercadoPago webhook sin payment id. No se ejecutará conciliación.");
                return Ok();
            }

            PaymentTransaction? paymentTransaction = await flexiRoomsContext.PaymentTransactions
                .FirstOrDefaultAsync(transaction => transaction.MercadoPagoPaymentId == paymentIdText, cancellationToken);

            if (paymentTransaction is null)
            {
                logger.LogWarning("No existe PaymentTransaction para MercadoPagoPaymentId {PaymentId}.", paymentIdText);
                return Ok();
            }

            // Marketplace: the payment lives in the hotel's (seller's) account, so it must be
            // read with that account's token. Legacy payments fall back to the platform token.
            SellerCredentials? seller = null;
            if (paymentTransaction.HotelId.HasValue)
            {
                seller = await mercadoPagoAccounts.GetCredentialsForHotelAsync(paymentTransaction.HotelId.Value, cancellationToken);
            }
            else if (long.TryParse(jsonPayment?.User_id, out long collectorId))
            {
                seller = await mercadoPagoAccounts.GetCredentialsForCollectorAsync(collectorId, cancellationToken);
            }

            var requestOptions = seller is null ? null : new MercadoPago.Client.RequestOptions { AccessToken = seller.AccessToken };

            MercadoPago.Resource.Payment.Payment payment;
            try
            {
                payment = await paymentClient.GetAsync(paymentId, requestOptions, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "No se pudo obtener el pago {PaymentId} desde Mercado Pago (hotel {HotelId}).", paymentIdText, paymentTransaction.HotelId);
                return Ok();
            }

            paymentTransaction.Status = payment.Status ?? paymentTransaction.Status;
            paymentTransaction.StatusDetail = payment.StatusDetail;

            bool isApproved = string.Equals(payment.Status, "approved", StringComparison.OrdinalIgnoreCase);
            if (!isApproved)
            {
                await flexiRoomsContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation(
                    "Webhook conciliado sin creación de reserva. PaymentId {PaymentId}, status {Status}, detail {StatusDetail}.",
                    paymentIdText,
                    payment.Status,
                    payment.StatusDetail);
                return Ok();
            }

            if (paymentTransaction.BookingId.HasValue)
            {
                bool bookingExists = await flexiRoomsContext.Bookings.AnyAsync(booking => booking.Id == paymentTransaction.BookingId.Value, cancellationToken);
                if (bookingExists)
                {
                    await flexiRoomsContext.SaveChangesAsync(cancellationToken);
                    logger.LogInformation(
                        "Webhook idempotente. PaymentId {PaymentId} ya tiene BookingId {BookingId}.",
                        paymentIdText,
                        paymentTransaction.BookingId.Value);
                    return Ok();
                }
            }

            if (!TryParseExternalReference(payment.ExternalReference, out int roomId, out DateTime dateReservedUtc, out int checkInTimeId))
            {
                await flexiRoomsContext.SaveChangesAsync(cancellationToken);
                logger.LogError(
                    "No se pudo interpretar ExternalReference para PaymentId {PaymentId}. ExternalReference: {ExternalReference}",
                    paymentIdText,
                    payment.ExternalReference);
                return Ok();
            }

            ApplicationUser? user = await userManager.Users.FirstOrDefaultAsync(user => user.Id == paymentTransaction.ApplicationUserId, cancellationToken);
            if (user is null)
            {
                await flexiRoomsContext.SaveChangesAsync(cancellationToken);
                logger.LogError(
                    "No se encontró usuario {UserId} para finalizar pago {PaymentId}.",
                    paymentTransaction.ApplicationUserId,
                    paymentIdText);
                return Ok();
            }

            Bookings? existingBooking = await flexiRoomsContext.Bookings.FirstOrDefaultAsync(
                booking => booking.UserGuid == user.UserGuid
                        && booking.IdRoom == roomId
                        && booking.CheckInTimeId == checkInTimeId
                        && booking.DateReserved == dateReservedUtc,
                cancellationToken);

            if (existingBooking is not null)
            {
                paymentTransaction.BookingId = existingBooking.Id;
                await flexiRoomsContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation(
                    "Webhook vinculó pago {PaymentId} con reserva existente {BookingId}.",
                    paymentIdText,
                    existingBooking.Id);
                return Ok();
            }

            Bookings bookingToCreate = new()
            {
                IdRoom = roomId,
                DateReserved = dateReservedUtc,
                CheckInTimeId = checkInTimeId,
                CostId = 0,
                UserGuid = user.UserGuid
            };

            try
            {
                Bookings createdBooking = await bookingService.Create(bookingToCreate);
                paymentTransaction.BookingId = createdBooking.Id;
                await flexiRoomsContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Webhook creó reserva {BookingId} para pago aprobado {PaymentId}.",
                    createdBooking.Id,
                    paymentIdText);
            }
            catch (Exception exception)
            {
                await flexiRoomsContext.SaveChangesAsync(cancellationToken);
                logger.LogError(exception,
                    "Error al crear/vincular reserva desde webhook. PaymentId {PaymentId}, RoomId {RoomId}, Date {DateReserved}, CheckInTimeId {CheckInTimeId}.",
                    paymentIdText,
                    roomId,
                    dateReservedUtc,
                    checkInTimeId);
            }

            return Ok();
        }

        private static long? TryGetPaymentId(HttpRequest request, string body)
        {
            if (request.Query.TryGetValue("data.id", out var paymentIdByDataId)
                && long.TryParse(paymentIdByDataId.ToString(), out long parsedDataId))
            {
                return parsedDataId;
            }

            if (request.Query.TryGetValue("id", out var paymentIdById)
                && long.TryParse(paymentIdById.ToString(), out long parsedId))
            {
                return parsedId;
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(body);

                if (document.RootElement.TryGetProperty("data", out JsonElement data)
                    && data.TryGetProperty("id", out JsonElement dataIdElement)
                    && long.TryParse(dataIdElement.ToString(), out long parsedBodyDataId))
                {
                    return parsedBodyDataId;
                }

                if (document.RootElement.TryGetProperty("id", out JsonElement idElement)
                    && long.TryParse(idElement.ToString(), out long parsedBodyId))
                {
                    return parsedBodyId;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static bool TryParseExternalReference(string? externalReference, out int roomId, out DateTime dateReservedUtc, out int checkInTimeId)
        {
            roomId = 0;
            checkInTimeId = 0;
            dateReservedUtc = default;

            if (string.IsNullOrWhiteSpace(externalReference))
            {
                return false;
            }

            string[] parts = externalReference.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 6)
            {
                return false;
            }

            if (!string.Equals(parts[0], "room", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(parts[2], "booking", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!int.TryParse(parts[1], out roomId)
                || !int.TryParse(parts[5], out checkInTimeId)
                || !DateTime.TryParseExact(parts[4], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return false;
            }

            dateReservedUtc = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            return true;
        }
    }
}
