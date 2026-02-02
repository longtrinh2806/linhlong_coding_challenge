using System.Security.Cryptography;
using OtpNet;
using Pharma.Identity.Application.Common.Abstractions;
using Constant = Pharma.Identity.Application.Constant;

namespace Pharma.Identity.Infrastructure.Services;

public class TotpService(IEncryptionService encryptionService) : ITotpService
{
    private const int SecretKeyLength = 32; // 32 bytes = 256 bits
    private const int BackupCodesCount = 10;
    
    public string GenerateSecretKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var secretKey = new byte[SecretKeyLength];
        rng.GetBytes(secretKey);
        return Base32Encoding.ToString(secretKey);
    }

    public string GenerateQrCodeUrl(string userEmail, string secretKey)
    {
        // Format: otpauth://totp/AppName:userEmail?secret=SECRET&issuer=AppName
        // This follows RFC 6238 standard for TOTP URLs
        var encodedEmail = Uri.EscapeDataString(userEmail);
        var encodedAppName = Uri.EscapeDataString(Constant.ApplicationName);

        return $"otpauth://totp/{encodedAppName}:{encodedEmail}?secret={secretKey}&issuer={encodedAppName}";
    }

    public string GenerateTotp(string secretKey)
    {
        var secretKeyBytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(secretKeyBytes);
        return totp.ComputeTotp();
    }

    public bool ValidateTotp(string secretKey, string totpCode)
    {
        var secretKeyBytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(secretKeyBytes);

        // Check current time window and ±1 time window (±30 seconds) for clock drift
        var currentTime = DateTime.UtcNow;

        // Check current window
        if (totp.ComputeTotp(currentTime) == totpCode)
        {
            return true;
        }

        // Check previous window (30 seconds ago)
        if (totp.ComputeTotp(currentTime.AddSeconds(-30)) == totpCode)
        {
            return true;
        }

        // Check next window (30 seconds ahead)
        if (totp.ComputeTotp(currentTime.AddSeconds(30)) == totpCode)
        {
            return true;
        }

        return false;
    }

    public List<string> GenerateBackupCodes()
    {
        var backupCodes = new List<string>();
        using var rng = RandomNumberGenerator.Create();

        for (var i = 0; i < BackupCodesCount; i++)
        {
            // Generate 8-digit backup codes in format: XXXX-XXXX
            var bytes = new byte[4];
            rng.GetBytes(bytes);

            var code1 = (BitConverter.ToUInt32(bytes, 0) % 10000).ToString("D4");

            rng.GetBytes(bytes);
            var code2 = (BitConverter.ToUInt32(bytes, 0) % 10000).ToString("D4");

            backupCodes.Add($"{code1}-{code2}");
        }

        return backupCodes;
    }

    public async Task<string> EncryptSecretKeyAsync(string secretKey, CancellationToken cancellationToken = default)
    {
        return await encryptionService.EncryptAsync(secretKey, cancellationToken);
    }

    public async Task<string> DecryptSecretKeyAsync(string encryptedSecretKey, CancellationToken cancellationToken = default)
    {
        return await encryptionService.DecryptAsync(encryptedSecretKey, cancellationToken);
    }
}