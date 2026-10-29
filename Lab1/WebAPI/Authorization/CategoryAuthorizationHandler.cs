using Application.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI.Authorization
{
    public class CategoryAuthorizationHandler : AuthorizationHandler<SameUserOrAdminRequirement, Category>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SameUserOrAdminRequirement requirement,
            Category resource)
        {
            // 1. Administrator ma pełny dostęp do zasobów wszystkich użytkowników
            if (context.User.IsInRole("Admin") || context.User.HasClaim("role", "Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 2. Pobranie ID zalogowanego użytkownika z tokenu JWT
            var userIdClaim = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                           ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 3. Weryfikacja, czy kategoria należy do tego użytkownika
            if (Guid.TryParse(userIdClaim, out var currentUserId) && resource.UserId == currentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
