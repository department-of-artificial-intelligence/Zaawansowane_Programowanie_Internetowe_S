using Application.Domain;

namespace Application.Queries.Dtos;

public class UserDto
{
    public Guid Id { get; }
    public string UserName { get; }
    public string Role { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly BirthDate { get; }
    public string Address { get; }

    private UserDto(
        Guid id,
        string userName,
        string role,
        string firstName,
        string lastName,
        DateOnly birthDate,
        string address)
    {
        Id = id;
        UserName = userName;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
    }

    public static UserDto FromDomain(User user)
        => new(
            user.Id,
            user.UserName,
            user.Role.ToString(),
            user.Profile.FirstName,
            user.Profile.LastName,
            user.Profile.BirthDate,
            user.Profile.Address);
}
