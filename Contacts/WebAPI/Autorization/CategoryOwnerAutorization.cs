using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Authorization;

public class CategoryOwnerRequirement : IAuthorizationRequirement { }

public class CategoryOwnerAuthorization(ICategoryRepository categoryRepository) 
    : AuthorizationHandler<CategoryOwnerRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CategoryOwnerRequirement requirement,
        Guid categoryId)
    {
        var userIdString = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdString, out var userId))
        {
            var categoryOption = categoryRepository.GetById(categoryId);

            categoryOption.Match(
                category =>
                {
                    var isAdmin = context.User.IsInRole("Admin") || context.User.HasClaim("role", "Admin");
                    if (category.UserId == userId || isAdmin)
                    {
                        context.Succeed(requirement);
                    }
                    return true;
                },
                () => false
            );
        }

        return Task.CompletedTask;
    }
}