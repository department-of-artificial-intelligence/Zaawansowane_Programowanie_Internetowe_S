using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contacts.Application.DomainServices;
namespace Contacts.WebAPI.Helpers;

public class TokenGenerator(IConfiguration configuration) : ITokenGenerator
{
    public string GenerateToken(int userId, string userName)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
        };
        var jwtToken = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddDays(30),
        signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ "ApplicationSettings:JWT_KEY" ])), SecurityAlgorithms.HmacSha256Signature));
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}