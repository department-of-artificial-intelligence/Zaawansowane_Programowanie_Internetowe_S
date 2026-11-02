using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.OutputPorts;
namespace WebAPI.Helpers;

public class TokenGenerator(IConfiguration configuration) : ITokenGenerator
{
    // Zadanie 10: krótko żyjący token dostępowy (zamiast 30 dni)
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);

    public string GenerateToken(Guid userId, string userName, string role)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Name, userName),
            new("role", role)
        };

        var jwtToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(AccessTokenLifetime),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["ApplicationSettings:JWT_KEY"]!)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    // Zadanie 10: token odświeżający to po prostu długi losowy ciąg (nie JWT)
    public string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    // W bazie zapisujemy tylko skrót SHA-256, więc wyciek bazy nie daje gotowych tokenów.
    // Skrót jest deterministyczny, więc można po nim wyszukać użytkownika.
    public string HashRefreshToken(string refreshToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}
