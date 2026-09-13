using Application.Models.User;
using Application.Models.Users;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<UserLoginDto> Authenticate(UserDto userDto);

        Task<bool> CreateAccount(UserCreateDto userCreateDto);

        Task<UserInformationDto?> GetInformation(string userId);

        Task<UserInformationDto?> UpdateInformation(string userId, UpdateUserInformationDto userInformationDto);

        Task<bool> UpdateProfileImage(string userId, byte[] content, string contentType, string fileName);

        Task<(byte[] Content, string ContentType, string FileName)?> GetProfileImage(string userId);
    }
}