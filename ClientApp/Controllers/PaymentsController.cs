using Application.Interfaces;
using Application.Models.Booking;

using Infrastructure.Models;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(
        IPayment payment,
        IRepository<Room> roomRepository,
        IMercadoPagoAccountService mercadoPagoAccounts,
        ILogger<PaymentsController> logger) : ControllerBase
    {
        /// <summary>
        /// Checkout Brick configuration for a room: the public key of the Mercado Pago
        /// account of the room's hotel (so the payment goes to that hotel) and the amount.
        /// </summary>
        [HttpGet("public-key")]
        public async Task<IActionResult> GetPublicKey(int roomId, int costId = 0, CancellationToken cancellationToken = default)
        {
            Room room = await roomRepository.GetByIdAsync(
                room => room.Id == roomId,
                query => query.Include(room => room.Costs).Include(room => room.Hotels));

            if (room is null)
                return NotFound(new { message = "No se encontró la habitación." });

            Cost? selectedCost = room.Costs?.FirstOrDefault(cost => cost.Id == costId) ?? room.Costs?.FirstOrDefault();
            if (selectedCost is null)
                return NotFound(new { message = "No se encontró el precio de la habitación." });

            if (room.Hotels is null)
                return NotFound(new { message = "La habitación no está asociada a un hotel." });

            if (room.Hotels.ApprovalStatus != HotelApprovalStatus.Approved)
                return Conflict(new { code = "hotel_not_approved", message = "Este hotel todavía no está habilitado para reservas online." });

            SellerCredentials? credentials = await mercadoPagoAccounts.GetCredentialsForHotelAsync(room.Hotels.Id, cancellationToken);
            if (credentials is null)
            {
                logger.LogWarning("Checkout requested for hotel {HotelId} without a connected Mercado Pago account.", room.Hotels.Id);
                return Conflict(new { code = "hotel_payments_not_configured", message = "Este hotel todavía no acepta pagos online." });
            }

            return Ok(new { publicKey = credentials.PublicKey, amount = selectedCost.CostPerTime });
        }

        [HttpPost]
        //[Authorize(Policy = "ReservationUser")]
        public async Task<IActionResult> CreatePayment([FromBody] CheckoutBooking checkout)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userGuidClaim = User.FindFirstValue("user_guid");
            if (!Guid.TryParse(userGuidClaim, out Guid userGuid))
                return Unauthorized();

            checkout.RoomDto.UserGuid = userGuid;

            try
            {
                var result = await payment.CreatePayment(checkout);
                logger.LogInformation("Pago aprobado por Mercado Pago: {PaymentId}", result.id);
                return Ok(new { paymentId = result.id, status = result.status });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}