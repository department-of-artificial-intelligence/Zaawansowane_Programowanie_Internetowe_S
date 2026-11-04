using Application.Domain;

namespace Application.UseCases.Dtos;

public class UserDto(Guid id, string userName, UserRole role, string firstName, string lastName, DateTime? birthDate, string address)
{
    public Guid Id { get; } = id;
    public string UserName { get; } = userName;
    public UserRole Role { get; } = role;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public DateTime? BirthDate { get; } = birthDate;
    public string Address { get; } = address;

    public static UserDto FromDomain(User user) =>
        new(user.Id, user.UserName, user.Role, user.FirstName, user.LastName, user.BirthDate, user.Address);
}
