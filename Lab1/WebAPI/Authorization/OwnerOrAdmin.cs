using System.Security.Claims;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using WebAPI.Helpers;
namespace WebAPI.Authorization;

// Zadanie 11: autoryzacja oparta o zasoby (resource-based authorization).
// Zasobem jest "coś, co ma właściciela" - tu kategoria.
public record OwnedResource(Guid OwnerId);

public sealed class OwnerOrAdminRequirement : IAuthorizationRequirement;

public sealed class OwnerOrAdminHandler
    : AuthorizationHandler<OwnerOrAdminRequirement, OwnedResource>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement,
        OwnedResource resource)
    {
        var isAdmin = context.User.IsInRole("Admin");
        var isOwner = context.User.Identity?.IsAuthenticated == true
            && context.User.GetUserId() == resource.OwnerId;

        if (isAdmin || isOwner)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

public static class OwnershipAuthorization
{
    public const string PolicyName = "OwnerOrAdmin";

    // Brak zasobu -> 404, cudzy zasób -> 403, właściciel lub admin -> wykonaj akcję
    public static Task<IResult> AuthorizeOwnerAsync(
        this IAuthorizationService authorizationService,
        ClaimsPrincipal user,
        Option<Guid> owner,
        string notFoundMessage,
        Func<IResult> onAuthorized) =>
        owner.Match(
            async ownerId =>
                (await authorizationService.AuthorizeAsync(
                    user, new OwnedResource(ownerId), PolicyName)).Succeeded
                    ? onAuthorized()
                    : Results.Forbid(),
            () => Task.FromResult(ResultHttpExtensions.NotFound(notFoundMessage)));
}
