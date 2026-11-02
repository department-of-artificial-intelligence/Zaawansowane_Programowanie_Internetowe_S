namespace Application.OutputPorts;

public interface ITokenGenerator
{
    string GenerateToken(Guid userId, string userName, string role);

    // Zadanie 10: losowy token odświeżający i jego skrót (w bazie trzymamy tylko skrót)
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
