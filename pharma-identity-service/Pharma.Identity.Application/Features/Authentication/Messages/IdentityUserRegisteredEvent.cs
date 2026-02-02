namespace Pharma.Identity.Application.Features.Authentication.Messages;

public record IdentityUserRegisteredEvent(string Email, string Otp);