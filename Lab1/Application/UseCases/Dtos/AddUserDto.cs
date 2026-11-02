using Application.Domain;
using Application.OutputPorts;
using Application.Utilities;
namespace Application.UseCases.Dtos;

public class AddUserDto(string userName, string password)
{
    public string UserName { get; } = userName;
    public string Password { get; } = password;

    // Zadanie 3: hasło przechodzi przez smart konstruktor
    public Result<User> ToDomain(IPasswordHasher hasher) =>
        User.Create(UserName, Password, hasher);
}
