namespace Pharma.Identity.Application.Features.Authentication.DTOs;

public record LoginResponse(string? AccessToken, string? RefreshToken, string? UserRole);