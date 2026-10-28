using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.OutputPorts;

namespace WebAPI.Helpers;

public class TokenGenerator(IConfiguration configuration, IClock clock) : ITokenGenerator
{
    public string GenerateToken(Guid userId, string userName, string role) {
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Name, userName),
            new("role", role)
        };
        // Zadanie 10 - token dostępowy jest ważny tylko kilkanaście minut.
        var jwtToken = new JwtSecurityToken(
            claims: claims,
            expires: clock.UtcNow.AddMinutes(
                configuration.GetValue<int>("ApplicationSettings:ACCESS_TOKEN_MINUTES")),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration[
                        "ApplicationSettings:JWT_KEY"
                    ]!)
                ),
                SecurityAlgorithms.HmacSha256)
            );
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    // Refresh token to losowy ciąg znaków (nie JWT) - jego ważność
    // potwierdza wyłącznie wpis w bazie danych.
    public RefreshToken GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return new RefreshToken(
            token,
            HashRefreshToken(token),
            clock.UtcNow.AddDays(
                configuration.GetValue<int>("ApplicationSettings:REFRESH_TOKEN_DAYS")));
    }

    // W bazie zapisywany jest tylko skrót - wyciek bazy nie ujawnia ważnych tokenów.
    // Token ma dużą entropię, więc wystarczy szybki SHA256 (bez soli).
    public string HashRefreshToken(string refreshToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}
