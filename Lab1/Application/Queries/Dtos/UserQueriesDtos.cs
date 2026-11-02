using Application.Domain;
namespace Application.Queries.Dtos;

// Bez skrótu hasła i tokenu odświeżającego - te dane nigdy nie wychodzą z API
public record UserDto(
    Guid Id,
    string UserName,
    UserRole Role,
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Address)
{
    public static UserDto FromDomain(User user) =>
        new(user.Id, user.UserName, user.Role,
            user.Profile.FirstName, user.Profile.LastName,
            user.Profile.BirthDate, user.Profile.Address);
}
