using System.Security.Claims;

namespace Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name)
               ?? user.FindFirstValue(CustomClaimTypes.Name);
    }
}