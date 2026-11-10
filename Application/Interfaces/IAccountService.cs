using Application.Models.User;
using Application.Models.Users;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<UserLoginDto> Authenticate(UserDto userDto);

        Task<bool> CreateAccount(UserCreateDto userCreateDto);

        /// <summary>
        /// Makes the user the owner of a hotel: adds the "Owner" role and the hotel to OwnedHotelIds.
        /// Returns a fresh login (new JWT with the role) so the panel opens without logging in again.
        /// </summary>
        Task<UserLoginDto> GrantHotelOwnershipAsync(string userId, int hotelId);

        Task<UserInformationDto?> GetInformation(string userId);

        Task<UserInformationDto?> UpdateInformation(string userId, UpdateUserInformationDto userInformationDto);

        Task<bool> UpdateProfileImage(string userId, byte[] content, string contentType, string fileName);

        Task<(byte[] Content, string ContentType, string FileName)?> GetProfileImage(string userId);
    }
}