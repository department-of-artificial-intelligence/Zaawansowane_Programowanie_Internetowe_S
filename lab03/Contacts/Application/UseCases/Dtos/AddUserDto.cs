using Application.Domain;
using Application.OutputPorts;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddUserDto(string userName, string password)
{
    public string UserName { get; } = userName;
    public string Password { get; } = password;

    public Result<User> ToDomain(IPasswordHasher passwordHasher) =>
        User.Create(UserName, Password, passwordHasher);
}
