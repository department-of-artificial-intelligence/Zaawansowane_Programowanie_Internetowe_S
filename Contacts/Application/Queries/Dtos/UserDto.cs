using Application.Domain;

namespace Application.Queries.Dtos;

public class UserDto
{
    public Guid Id { get; }
    public string UserName { get; }
    public string Role { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime? DateOfBirth { get; }
    public string Address { get; }

    private UserDto(
        Guid id, 
        string userName, 
        string role, 
        string firstName, 
        string lastName, 
        DateTime? dateOfBirth, 
        string address)
    {
        Id = id;
        UserName = userName;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address;
    }

    public static UserDto FromDomain(User user) =>
        new(
            user.Id,
            user.UserName,
            user.Role.ToString(),
            user.FirstName.Value,
            user.LastName.Value,
            user.DateOfBirth,
            user.Address
        );
}