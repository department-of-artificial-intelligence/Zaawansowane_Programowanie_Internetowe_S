namespace Application.UseCases.Dtos;

public class LoginUserResult(string accessToken = "", string refreshToken = "")
{
    public string AccessToken { get; } = accessToken;
    public string RefreshToken { get; } = refreshToken;
}
