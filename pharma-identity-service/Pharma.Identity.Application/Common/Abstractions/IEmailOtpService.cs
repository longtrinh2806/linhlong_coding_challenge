namespace Pharma.Identity.Application.Common.Abstractions;

public interface IEmailOtpService
{
    Task<string> GenerateEmailOtpAsync(string email, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    Task<bool> VerifyEmailOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
}