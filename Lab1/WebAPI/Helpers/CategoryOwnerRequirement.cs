using Application.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Helpers;

public sealed class CategoryOwnerRequirement : IAuthorizationRequirement;

public sealed class CategoryOwnerHandler : AuthorizationHandler<CategoryOwnerRequirement, Category>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CategoryOwnerRequirement requirement,
        Category resource)
    {
        var isAdmin = context.User.IsInRole("Admin");
        var ownsCategory = Guid.TryParse(
            context.User.FindFirst("sub")?.Value,
            out var userId) && resource.UserId == userId;

        if (isAdmin || ownsCategory)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
