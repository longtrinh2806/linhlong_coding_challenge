namespace Pharma.Identity.Application.Features.Authentication.DTOs;

public record ValidateOtpRequest(string Email, string Otp);