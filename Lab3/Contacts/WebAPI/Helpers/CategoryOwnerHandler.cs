using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Helpers;

public class CategoryOwnerHandler
    : AuthorizationHandler<CategoryOwnerRequirement, Guid>
{

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CategoryOwnerRequirement requirement,
        Guid ownerId)
    {
        var userId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (context.User.IsInRole("Admin") || userId == ownerId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
