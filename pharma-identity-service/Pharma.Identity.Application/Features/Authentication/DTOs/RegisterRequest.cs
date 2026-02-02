namespace Pharma.Identity.Application.Features.Authentication.DTOs;

public class RegisterRequest
{
    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string ConfirmPassword { get; set; }
}