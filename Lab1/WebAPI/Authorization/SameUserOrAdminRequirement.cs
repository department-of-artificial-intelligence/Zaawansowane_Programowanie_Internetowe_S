using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Authorization
{
    public class SameUserOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
