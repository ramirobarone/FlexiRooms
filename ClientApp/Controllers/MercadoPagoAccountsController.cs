using System.Security.Claims;
using Application.Interfaces;
using Application.Models.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClientApp.Controllers
{
    /// <summary>
    /// Connects each hotel's Mercado Pago seller account (Marketplace OAuth) so that
    /// payments for its rooms are credited directly to the hotel.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoPagoAccountsController(
        IMercadoPagoAccountService accountService,
        IOptions<MercadoPagoMarketplaceOptions> options,
        ILogger<MercadoPagoAccountsController> logger) : ControllerBase
    {
        /// <summary>Payment-account status of every hotel owned by the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyStatuses(CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var statuses = await accountService.GetStatusesForOwnerAsync(userId, cancellationToken);
            return Ok(new { marketplaceConfigured = accountService.IsMarketplaceConfigured, hotels = statuses });
        }

        [HttpGet("{hotelId:int}")]
        public async Task<IActionResult> GetStatus(int hotelId, CancellationToken cancellationToken)
        {
            if (!await CanManage(hotelId, cancellationToken))
                return Forbid();

            return Ok(await accountService.GetStatusAsync(hotelId, cancellationToken));
        }

        /// <summary>Starts the OAuth flow: returns the Mercado Pago URL the browser must open.</summary>
        [HttpPost("{hotelId:int}/connect")]
        public async Task<IActionResult> Connect(int hotelId, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            if (!await CanManage(hotelId, cancellationToken))
                return Forbid();

            if (!accountService.IsMarketplaceConfigured)
                return Problem("Mercado Pago Marketplace no está configurado en el servidor.", statusCode: StatusCodes.Status503ServiceUnavailable);

            return Ok(new { authorizationUrl = accountService.BuildAuthorizationUrl(hotelId, userId) });
        }

        [HttpDelete("{hotelId:int}")]
        public async Task<IActionResult> Disconnect(int hotelId, CancellationToken cancellationToken)
        {
            if (!await CanManage(hotelId, cancellationToken))
                return Forbid();

            await accountService.DisconnectAsync(hotelId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// OAuth redirect_uri registered in the Mercado Pago application. It is reached by a
        /// browser redirect (no bearer token), so identity comes from the signed state.
        /// Always redirects back to the admin panel with the outcome in the query string.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("oauth/callback")]
        public async Task<IActionResult> OAuthCallback(
            [FromQuery] string? code,
            [FromQuery] string? state,
            [FromQuery] string? error,
            CancellationToken cancellationToken)
        {
            int? hotelId = accountService.TryReadHotelIdFromState(state);

            if (!string.IsNullOrWhiteSpace(error) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            {
                logger.LogWarning("Mercado Pago OAuth callback without code (error: {Error}).", error);
                return RedirectToAdmin("cancelled", hotelId);
            }

            try
            {
                int connectedHotelId = await accountService.CompleteAuthorizationAsync(code, state, cancellationToken);
                return RedirectToAdmin("connected", connectedHotelId);
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogWarning(exception, "Mercado Pago OAuth callback rejected for hotel {HotelId}.", hotelId);
                return RedirectToAdmin("forbidden", hotelId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Mercado Pago OAuth callback failed for hotel {HotelId}.", hotelId);
                return RedirectToAdmin("error", hotelId);
            }
        }

        private IActionResult RedirectToAdmin(string result, int? hotelId)
        {
            string url = $"{options.Value.AdminReturnUrl}?mp={Uri.EscapeDataString(result)}";
            if (hotelId.HasValue)
                url += $"&hotelId={hotelId.Value}";

            return Redirect(url);
        }

        private Task<bool> CanManage(int hotelId, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Task.FromResult(false);

            return accountService.CanManageHotelAsync(hotelId, userId, User.IsInRole("SuperAdmin"), cancellationToken);
        }
    }
}
