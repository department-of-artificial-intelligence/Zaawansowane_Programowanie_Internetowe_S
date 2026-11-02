using Application.Domain;
using Application.Utilities;
using Application.OutputPorts;
namespace Application.UseCases.Dtos;
public class AddUserDto(string userName, string password, string firstName, string lastName, DateTime? dateOfBirth = null, string address = "")
{
    public string UserName { get; } = userName;
    public string Password { get; } = password;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public DateTime? DateOfBirth { get; } = dateOfBirth;
    public string Address { get; } = address;
    public Result<User> ToDomain(IPasswordHasher passwordHasher) => 
        Domain.FirstName.Create(FirstName)
            .Bind(fName => Domain.LastName.Create(LastName)
            .Bind(lName => User.Create(UserName, Password, passwordHasher, fName, lName, DateOfBirth, Address)));
}
