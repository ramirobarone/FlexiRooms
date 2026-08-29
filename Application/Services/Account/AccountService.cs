using Application.Interfaces;
using Application.Models.User;
using Application.Models.Users;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Account
{
    public class AccountService(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService) : IAccountService
    {
        public async Task<UserLoginDto> Authenticate(UserDto userDto)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            ApplicationUser? user = await userManager.FindByEmailAsync(userDto.Email);
            if (user is null)
            {
                throw new UnauthorizedAccessException("The user is not authorizated");
            }

            bool isValidPassword = await userManager.CheckPasswordAsync(user, userDto.Password);
            if (!isValidPassword)
            {
                throw new UnauthorizedAccessException("The user is not authorizated");
            }

            IList<string> roles = await userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? "User";
            string token = await jwtTokenService.CreateTokenAsync(user);
            string fullName = $"{user.Name} {user.LastName}".Trim();

            return new UserLoginDto(
                string.IsNullOrWhiteSpace(fullName) ? user.Email ?? string.Empty : fullName,
                token,
                0,
                role,
                user.UserGuid);
        }

        public async Task<bool> CreateAccount(UserCreateDto userCreateDto)
        {
            ArgumentNullException.ThrowIfNull(userCreateDto);

            ApplicationUser? existingUser = await userManager.FindByEmailAsync(userCreateDto.Email);
            if (existingUser is not null)
            {
                return false;
            }

            userCreateDto.CreateUserGuid();

            ApplicationUser user = new()
            {
                UserName = userCreateDto.Email,
                Email = userCreateDto.Email,
                Name = userCreateDto.Name,
                LastName = userCreateDto.LastName,
                PhoneNumber = userCreateDto.PhoneNumber,
                IdentityNumber = userCreateDto.IdentityNumber,
                AccountActivate = true,
                CodeArea = userCreateDto.CodeArea,
                UserGuid = userCreateDto.UserGuid ?? Guid.NewGuid(),
                EmailConfirmed = true
            };

            IdentityResult createdResult = await userManager.CreateAsync(user, userCreateDto.Password);

            if (!createdResult.Succeeded)
            {
                return false;
            }

            IdentityResult roleResult = await userManager.AddToRoleAsync(user, "User");
            return roleResult.Succeeded;
        }
    }
}
