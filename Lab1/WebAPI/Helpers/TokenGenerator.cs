using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.OutputPorts;

namespace WebAPI.Helpers;

public class TokenGenerator(IConfiguration configuration) : ITokenGenerator{

	public string GenerateToken(Guid userId, string userName, string role){
		var claims = new List<Claim> {
			new(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new(JwtRegisteredClaimNames.Name, userName),
			new("role", role)
		};

		var jwtToken = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.UtcNow.AddDays(30),
			signingCredentials: new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ApplicationSettings:JWT_KEY"]!)),
				SecurityAlgorithms.HmacSha256
			)
		);

		return new JwtSecurityTokenHandler().WriteToken(jwtToken);
	}
}
