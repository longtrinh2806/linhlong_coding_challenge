namespace Pharma.Identity.Application.Features.Authentication.DTOs;

public class LoginRequest
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}