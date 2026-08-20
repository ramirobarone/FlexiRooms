using Application.Interfaces;
using Application.Models.User;
using Application.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAccountService accountService, ILogger<AuthController> logger) : ControllerBase
    {
        [ProducesResponseType(typeof(UserLoginDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost(nameof(Authenticat))]
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
    }
}
