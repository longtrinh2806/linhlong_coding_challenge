using Pharma.Identity.Application.Common.Abstractions;

namespace Pharma.Identity.Infrastructure.Services;

public class EmailOtpService(ICachingService cachingService) : IEmailOtpService
{
    private const int DefaultOtpExpirationMinutes = 1;
    private readonly Random _random = new();

    public async Task<string> GenerateEmailOtpAsync(string email, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var otp = GenerateRandomOtp();
        var cachedKey = $"otp:{email}";

        await cachingService.SetAsync(
            key: cachedKey,
            value: otp,
            expiration: expiration ?? TimeSpan.FromMinutes(DefaultOtpExpirationMinutes),
            cancellationToken: cancellationToken
        );

        return otp;
    }

    public async Task<bool> VerifyEmailOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        var cachedKey = $"otp:{email}";
        var cachedOtp = await cachingService.GetAsync<string>( 
            key: cachedKey,
            cancellationToken: cancellationToken
        );

        return cachedOtp != null && cachedOtp == otp;
    }

    private string GenerateRandomOtp()
    {
        return _random.Next(100000, 999999).ToString();
    }
}