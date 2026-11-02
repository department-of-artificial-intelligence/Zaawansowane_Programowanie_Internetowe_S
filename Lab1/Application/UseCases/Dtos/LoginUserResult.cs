namespace Application.UseCases.Dtos;

public class LoginUserResult(string accessToken, string refreshToken)
{
    public string AccessToken { get; } = accessToken;

    // Zadanie 10
    public string RefreshToken { get; } = refreshToken;
}
