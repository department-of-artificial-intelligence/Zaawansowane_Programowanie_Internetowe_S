using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Helpers;

public class CategoryOwnerOrAdminRequirement : IAuthorizationRequirement;

public class CategoryAuthorizationHandler : AuthorizationHandler<CategoryOwnerOrAdminRequirement, Category>
{
    protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, CategoryOwnerOrAdminRequirement requirement, Category resource)
    {
        var userId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (context.User.IsInRole("Admin") || resource.UserId.ToString() == userId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}