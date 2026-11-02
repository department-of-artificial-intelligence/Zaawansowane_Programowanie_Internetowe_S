namespace Application.UseCases.Dtos;

public class TokenResponseDto(string accessToken, string refreshToken)
{
    public string AccessToken { get; } = accessToken;
    public string RefreshToken { get; } = refreshToken;
}