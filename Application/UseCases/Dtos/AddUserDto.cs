
using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddUserDto(string username, string password)
{
    public string UserName { get; } = username;
    public string Password { get; } = password;

    public Result<User> ToDomain() => User.Create(UserName);
}