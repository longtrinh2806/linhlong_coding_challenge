namespace Pharma.Identity.Application.Features.Authentication.DTOs;

public record LoginResponse(bool RequiresTwoFactor, string? AccessToken, string? RefreshToken, string? TwoFactorToken);