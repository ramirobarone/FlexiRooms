using Application.Models.User;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Models;

namespace Application.Models.Users
{
#pragma warning disable
    public class UserCreateDto : UserDto
    {

        public UserCreateDto(string name, string lastName, string codeArea, string phoneNumber, string identityNumber, string email, string password) : base(email, password)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            CodeArea = codeArea ?? throw new ArgumentNullException(nameof(codeArea));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            IdentityNumber = identityNumber ?? throw new ArgumentNullException(nameof(identityNumber));
        }

        [MinLength(3)]
        public required string Name { get; init; }
        [MinLength(3)]
        public required string LastName { get;  init; }
        [MinLength(3)]
        public string CodeArea { get; init; }
        [MinLength(7)]
        public string PhoneNumber { get; init; }
        [MinLength(8)]
        public string IdentityNumber { get; init; }
        public Guid? UserGuid { get; private set; }

        public static implicit operator Infrastructure.Models.User(UserCreateDto userCreateDto)
        {
            return new Infrastructure.Models.User()
            {
                Email = userCreateDto.Email,
                Password = userCreateDto.Password,
                PhoneNumber = userCreateDto.PhoneNumber,
                IdentityNumber = userCreateDto.IdentityNumber,
                AccountActivate = false,
                CodeArea = userCreateDto.CodeArea,
                LastName = userCreateDto.LastName,
                Name = userCreateDto.Name
            };
        }
        public void CreateUserGuid()
        {
            if (UserGuid == null)
                UserGuid = Guid.NewGuid();
        }
    }
}
