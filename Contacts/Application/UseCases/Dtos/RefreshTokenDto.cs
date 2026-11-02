namespace Application.UseCases.Dtos;

public class RefreshTokenDto(string refreshToken)
{
    public string RefreshToken { get; } = refreshToken;
}