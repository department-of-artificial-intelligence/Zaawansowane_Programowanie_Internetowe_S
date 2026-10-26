namespace Application.UseCases.Dtos;
public class LoginUserDto(string userName, string password)
{
    public string UserName { get; } = userName;
    public string Password {get; } = password;
}
