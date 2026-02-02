using System.Security.Claims;

namespace Pharma.Identity.Application.Common.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(Ulid userId, string email);
    string GenerateRefreshToken(Ulid userId, string email, TimeSpan? expiration = null);
    ClaimsPrincipal? ValidateRefreshToken(string refreshToken);
}