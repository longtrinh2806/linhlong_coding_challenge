namespace Pharma.Identity.Application.Common.Abstractions;

public interface IJwtTokenConfiguration
{
    string Issuer { get; }
    string Audience { get; }
    string SecretKey { get; }
    int AccessTokenExpirationMinutes { get; }
    int RefreshTokenExpirationDays { get; }
}