using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using Application.Domain;
using Application.Queries.Dtos;

namespace WebAPI.Authorization;

// Zadanie 11 - autoryzacja oparta o zasoby: kategorię może modyfikować
// i usuwać tylko jej właściciel lub administrator.
public class CategoryOwnerRequirement : IAuthorizationRequirement;

public class CategoryOwnerHandler
    : AuthorizationHandler<CategoryOwnerRequirement, CategoryDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CategoryOwnerRequirement requirement,
        CategoryDto category)
    {
        var isOwner = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            == category.UserId.ToString();

        if (isOwner || context.User.IsInRole(nameof(UserRole.Admin)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
