using Application.Interfaces;
using Application.Models.User;
using Application.Models.Users;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Account
{
    public class AccountService(UserManager<ApplicationUser> userManager,
                                IJwtTokenService jwtTokenService,
                                FlexiRoomsContext flexiRoomsContext) : IAccountService
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

        public async Task<UserInformationDto?> GetInformation(string userId)
        {
            ApplicationUser? user = await userManager.Users
                .Include(currentUser => currentUser.UserProfileImage)
                .FirstOrDefaultAsync(currentUser => currentUser.Id == userId);

            return user is null ? null : ToInformationDto(user);
        }

        public async Task<UserInformationDto?> UpdateInformation(string userId, UpdateUserInformationDto userInformationDto)
        {
            ArgumentNullException.ThrowIfNull(userInformationDto);

            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return null;
            }

            user.Name = userInformationDto.Name;
            user.SecondName = userInformationDto.SecondName;
            user.LastName = userInformationDto.LastName;
            user.IdentityNumber = userInformationDto.IdentityNumber;
            user.CodeArea = userInformationDto.CodeArea;
            user.PhoneNumber = userInformationDto.PhoneNumber;

            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
            }

            return ToInformationDto(user);
        }

        public async Task<bool> UpdateProfileImage(string userId, byte[] content, string contentType, string fileName)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return false;
            }

            UserProfileImage? profileImage = await flexiRoomsContext.UserProfileImages.FindAsync(userId);
            if (profileImage is null)
            {
                profileImage = new UserProfileImage { UserId = userId };
                flexiRoomsContext.UserProfileImages.Add(profileImage);
            }

            profileImage.Content = content;
            profileImage.ContentType = contentType;
            profileImage.FileName = fileName;
            profileImage.UpdatedAtUtc = DateTime.UtcNow;
            await flexiRoomsContext.SaveChangesAsync();
            return true;
        }

        public async Task<(byte[] Content, string ContentType, string FileName)?> GetProfileImage(string userId)
        {
            UserProfileImage? profileImage = await flexiRoomsContext.UserProfileImages
                .AsNoTracking()
                .FirstOrDefaultAsync(image => image.UserId == userId);

            return profileImage is null
                ? null
                : (profileImage.Content, profileImage.ContentType, profileImage.FileName);
        }

        private static UserInformationDto ToInformationDto(ApplicationUser user)
        {
            string? profileImageUrl = user.UserProfileImage is null
                ? null
                : $"/api/Auth/profile-image/{user.Id}?v={user.UserProfileImage.UpdatedAtUtc.Ticks}";

            return new UserInformationDto(
                user.Id,
                user.Email,
                user.Name,
                user.SecondName,
                user.LastName,
                user.IdentityNumber,
                user.CodeArea,
                user.PhoneNumber,
                profileImageUrl);
        }
    }
}
