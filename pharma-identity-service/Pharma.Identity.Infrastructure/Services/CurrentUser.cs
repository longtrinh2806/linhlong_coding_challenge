using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Pharma.Identity.Application.Common.Abstractions;

namespace Pharma.Identity.Infrastructure.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Ulid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;

            if (userIdClaim != null && Ulid.TryParse(userIdClaim, out var ulid))
            {
                return ulid;
            }

            return Ulid.Empty;
        }
    }

    public string? UserEmail => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? null;
}