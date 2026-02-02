namespace Pharma.Identity.Application.Features.Authentication.DTOs;

public class RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}