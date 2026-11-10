using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ClientApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController(IHotelService serviceHotel, IAccountService accountService, ILogger<HotelController> logger) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet(nameof(GetHotels))]
        public async Task<IActionResult> GetHotels(string searchKey)
        {
            logger.LogInformation("NameMethod {nameof(GetHotels)} - searchKey: {searchKey}", nameof(GetHotels), searchKey);

            if (string.IsNullOrEmpty(searchKey))
                return NoContent();

            IEnumerable<HotelDto>? resultHotels = await serviceHotel.SearchByKeyword(searchKey);
            logger.LogInformation("NameMethod {nameof(GetHotels)} - ResultSearchKey: {resultHotels}", nameof(GetHotels), System.Text.Json.JsonSerializer.Serialize(resultHotels));

            if (resultHotels.Any())
                return Ok(resultHotels);

            return NoContent();
        }
        

        [HttpGet(nameof(GetHotel))]
        public async Task<IActionResult> GetHotel([FromQuery] int idHotel)
        {
            logger.LogInformation("NameMethod {GetHotel} IdHotel {idHotel}", nameof(GetHotel), idHotel);

            if (idHotel <= 0)
                return NoContent();

            HotelDto _hotelDto = await serviceHotel.GetById(idHotel);

            logger.LogInformation("Service Name: {GetHotel} - Result: {_hotelDto}", nameof(serviceHotel.GetById), System.Text.Json.JsonSerializer.Serialize(_hotelDto));

            if (_hotelDto == null)
                return NoContent();

            return Ok(_hotelDto);
        }
        /// <summary>
        /// Self-service sign-up ("Sumá tu hotel") and "Nuevo hotel" in the panel.
        /// The creator becomes the owner (role Owner + OwnedHotelIds). Hotels created by a
        /// non-SuperAdmin start Pending: hidden from search and without payments until approved.
        /// Returns a fresh login so the new Owner role is in the token right away.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [HttpPost(nameof(CreateHotel))]
        public async Task<IActionResult> CreateHotel(HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(hotelDto.Name))
                return BadRequest(new { message = "El nombre del hotel es obligatorio." });

            bool isSuperAdmin = User.IsInRole("SuperAdmin");
            HotelDto created = await serviceHotel.CreateForOwner(hotelDto, userId, approveImmediately: isSuperAdmin);
            var login = await accountService.GrantHotelOwnershipAsync(userId, created.Id);

            return Created($"/api/Hotel/GetHotel?idHotel={created.Id}", new
            {
                hotelId = created.Id,
                approvalStatus = created.ApprovalStatus,
                login
            });
        }

        /// <summary>SuperAdmin review queue.</summary>
        [Authorize(Policy = "SuperAdminOnly")]
        [HttpGet(nameof(GetPendingHotels))]
        public async Task<IActionResult> GetPendingHotels() => Ok(await serviceHotel.GetPendingHotels());

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> ApproveHotel(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return await serviceHotel.ReviewHotel(id, approve: true, rejectionReason: null, userId) ? NoContent() : NotFound();
        }

        public sealed record RejectHotelRequest(string? Reason);

        [Authorize(Policy = "SuperAdminOnly")]
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> RejectHotel(int id, [FromBody] RejectHotelRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "Indicá el motivo del rechazo para que el dueño pueda corregirlo." });

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return await serviceHotel.ReviewHotel(id, approve: false, request.Reason, userId) ? NoContent() : NotFound();
        }
        [HttpGet(nameof(GetMyHotels))]
        public async Task<IActionResult> GetMyHotels()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            IEnumerable<HotelDto> myHotels = await serviceHotel.GetMyHotels(userId);

            if (!myHotels.Any())
                return NoContent();

            return Ok(myHotels);
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetHomeHotels))]
        public async Task<IActionResult> GetHomeHotels([FromQuery] int pageNumber = 1)
        {
            IEnumerable<HotelDto> homeHotels = await serviceHotel.GetHomeHotels(pageNumber);

            if (!homeHotels.Any())
                return NoContent();

            return Ok(homeHotels);
        }

        [HttpPut(nameof(UpdateHotel))]
        public async Task<IActionResult> UpdateHotel(HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            HotelOwnership? ownership = await serviceHotel.GetOwnership(hotelDto.Id);
            if (ownership is null)
                return NotFound();

            if (!ownership.CanBeManagedBy(userId, User.IsInRole("SuperAdmin")))
            {
                logger.LogWarning("User {UserId} tried to update hotel {HotelId} without permission.", userId, hotelDto.Id);
                return Forbid();
            }

            // The owner is never taken from the request body: it can't be reassigned or cleared by an update.
            // (The review state can't change here either: the service ignores it on update.)
            hotelDto.IdentityNumber = ownership.OwnerUserId;

            await serviceHotel.Update(hotelDto);

            // A rejected hotel goes back to the review queue once the owner fixes and saves it.
            if (ownership.ApprovalStatus == Infrastructure.Models.HotelApprovalStatus.Rejected)
                await serviceHotel.Resubmit(hotelDto.Id);

            return Ok();
        }

        [HttpDelete(nameof(DeleteHotel))]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            HotelOwnership? ownership = await serviceHotel.GetOwnership(id);
            if (ownership is null)
                return NotFound();

            if (!ownership.CanBeManagedBy(userId, User.IsInRole("SuperAdmin")))
            {
                logger.LogWarning("User {UserId} tried to delete hotel {HotelId} without permission.", userId, id);
                return Forbid();
            }

            await serviceHotel.Delete(id);
            return Accepted();
        }

    }
}