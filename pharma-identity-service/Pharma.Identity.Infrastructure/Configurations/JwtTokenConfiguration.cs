using Pharma.Identity.Application.Common.Abstractions;

namespace Pharma.Identity.Infrastructure.Configurations;

public class JwtTokenConfiguration : IJwtTokenConfiguration
{
    public required string Audience { get; init; }    
    public required string Issuer { get; init; }
    public required string SecretKey { get; init; }
    public required int AccessTokenExpirationMinutes { get; init; }
    public required int RefreshTokenExpirationDays { get; init; }
}