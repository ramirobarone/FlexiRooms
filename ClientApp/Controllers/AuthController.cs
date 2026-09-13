using Application.Interfaces;
using Application.Models.User;
using Application.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAccountService accountService, ILogger<AuthController> logger) : ControllerBase
    {
        [ProducesResponseType(typeof(UserLoginDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost(nameof(Authenticat))]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticat([FromBody] UserDto userDto)
        {
            logger.LogInformation("Authenticating user {Email}", userDto.Email);

            try
            {
                UserLoginDto loginUser = await accountService.Authenticate(userDto);
                logger.LogInformation("User {Email} authenticated successfully with role {Role}", userDto.Email, loginUser.Role);
                return Ok(loginUser);
            }
            catch (UnauthorizedAccessException)
            {
                logger.LogWarning("Authentication failed for user {Email}", userDto.Email);
                return Unauthorized();
            }
        }

        [HttpPost(nameof(CreateAccount))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount([FromBody] UserCreateDto userCreateDto)
        {
            logger.LogInformation("Creating account for {Email}", userCreateDto.Email);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model received in CreateAccount for {Email}", userCreateDto.Email);
                return BadRequest(ModelState);
            }

            bool accountCreated = await accountService.CreateAccount(userCreateDto);
            if (accountCreated)
            {
                logger.LogInformation("Account created successfully for {Email}", userCreateDto.Email);
                return Created("/home", accountCreated);
            }

            logger.LogInformation("Account could not be created because email already exists or role assignment failed for {Email}", userCreateDto.Email);
            return Ok(accountCreated);
        }

        [HttpGet(nameof(GetInformation))]
        [Authorize]
        [ProducesResponseType(typeof(UserInformationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInformation()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            UserInformationDto? information = await accountService.GetInformation(userId);
            return information is null ? NotFound() : Ok(information);
        }

        [HttpPut(nameof(UpdateInformation))]
        [Authorize]
        [ProducesResponseType(typeof(UserInformationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInformation([FromBody] UpdateUserInformationDto userInformationDto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            UserInformationDto? information = await accountService.UpdateInformation(userId, userInformationDto);
            return information is null ? NotFound() : Ok(information);
        }

        [HttpPost(nameof(UpdateProfileImage))]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateProfileImage([FromForm] IFormFile file)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (file is null || file.Length == 0 || file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("The profile image must be between 1 byte and 5 MB.");
            }

            string[] allowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
            if (!allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("Only JPEG, PNG and WebP images are supported.");
            }

            await using MemoryStream stream = new();
            await file.CopyToAsync(stream);
            bool updated = await accountService.UpdateProfileImage(
                userId,
                stream.ToArray(),
                file.ContentType,
                Path.GetFileName(file.FileName));

            return updated ? NoContent() : NotFound();
        }

        [HttpGet("profile-image/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfileImage(string userId)
        {
            (byte[] Content, string ContentType, string FileName)? image = await accountService.GetProfileImage(userId);
            return image is null ? NotFound() : File(image.Value.Content, image.Value.ContentType, image.Value.FileName);
        }
    }
}
