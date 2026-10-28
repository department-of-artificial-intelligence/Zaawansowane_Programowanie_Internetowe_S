namespace Application.OutputPorts;

public interface ITokenGenerator
{
    string GenerateToken(Guid userId, string userName, string role);

    // Zadanie 10 - refresh token oraz skrót, pod którym jest zapisywany w bazie.
    RefreshToken GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

public record RefreshToken(string Token, string TokenHash, DateTime ExpiresAt);
