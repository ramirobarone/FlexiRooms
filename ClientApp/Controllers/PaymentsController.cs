using Application.Interfaces;
using Application.Models.Booking;
using ClientApp.OptionsPattern;
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
        IConfiguration configuration,
        ILogger<PaymentsController> logger) : ControllerBase
    {
        [HttpGet("public-key")]
        public async Task<IActionResult> GetPublicKey(int roomId)
        {
            string? publicKey = configuration.GetSection(MercadoPagoOption.MercadoPagoOptionName).Get<MercadoPagoOption>()?.PublicKey;
            if (string.IsNullOrWhiteSpace(publicKey))
                return Problem("Mercado Pago no está configurado.");

            Room room = await roomRepository.GetByIdAsync(room => room.Id == roomId, query => query.Include(room => room.Cost));
            if (room?.Cost is null)
                return NotFound(new { message = "No se encontró el precio de la habitación." });

            return Ok(new { publicKey, amount = room.Cost.CostPerTime });
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