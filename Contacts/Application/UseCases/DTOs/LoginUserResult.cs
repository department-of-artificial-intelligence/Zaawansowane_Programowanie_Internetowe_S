namespace Contacts.Application.UseCases.DTOs;

public enum UserLoginStatus
{
    WrongUserOrPassword,
    UserLogged,
}

public class LoginUserResult
{
    public UserLoginStatus Status { get; init; }
    public string Token { get; init; } = string.Empty;
}
