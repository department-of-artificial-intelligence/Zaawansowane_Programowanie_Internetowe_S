using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace WebAPI.Helpers;

public static class ClaimsPrincipalExtensions
{
    // Identyfikator zalogowanego użytkownika zawsze z tokenu (claim "sub"), nigdy od klienta
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}
