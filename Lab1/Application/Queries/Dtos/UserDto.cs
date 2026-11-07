using Application.Domain;

namespace Application.Queries.Dtos;

public record UserDto(Guid Id, string UserName, string Role, string? FirstName, string? LastName,
    DateOnly? BirthDate, string? Address){
    public static UserDto FromDomain(User user) => new(user.Id, user.UserName, user.Role.ToString(), user.FirstName, user.LastName,
        user.BirthDate, user.Address);
}
