using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Commands.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _accessor;
    public UserAccessor(IHttpContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentException("Valid IHttpContextAccessor is needed", nameof(accessor));
    }

    public ClaimsPrincipal User => _accessor.HttpContext?.User!;
}

public interface IUserAccessor
{
    ClaimsPrincipal User { get; }
}
