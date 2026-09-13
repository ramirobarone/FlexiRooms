namespace Application.Models.Users;

public record UserInformationDto(
    string Id,
    string? Email,
    string? Name,
    string? SecondName,
    string? LastName,
    string? IdentityNumber,
    string? CodeArea,
    string? PhoneNumber,
    string? ProfileImageUrl);
