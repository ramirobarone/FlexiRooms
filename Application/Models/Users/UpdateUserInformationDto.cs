namespace Application.Models.Users;

public record UpdateUserInformationDto(
    string? Name,
    string? SecondName,
    string? LastName,
    string? IdentityNumber,
    string? CodeArea,
    string? PhoneNumber);
