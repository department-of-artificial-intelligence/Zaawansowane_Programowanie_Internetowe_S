using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Helpers;

public static class ClaimsPrincipalExtensions
{
    // Identyfikator zalogowanego użytkownika zawsze pochodzi z tokenu,
    // nigdy z treści żądania, query stringu czy trasy (ochrona przed IDOR).
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}
