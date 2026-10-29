using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Queries.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Helpers;

// 1. Definicja wymagania autoryzacyjnego
public class SameOwnerRequirement : IAuthorizationRequirement { }

// 2. Handler sprawdzający, czy użytkownik jest właścicielem zasobu lub administratorem
public class CategoryAuthorizationHandler : AuthorizationHandler<SameOwnerRequirement, CategoryDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameOwnerRequirement requirement,
        CategoryDto resource)
    {
        // Administrator ma zawsze pełne uprawnienia
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Odczytanie Id użytkownika z tokena JWT
        var userIdClaim = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                          ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is not null && Guid.TryParse(userIdClaim, out var userId))
        {
            if (resource.UserId == userId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}