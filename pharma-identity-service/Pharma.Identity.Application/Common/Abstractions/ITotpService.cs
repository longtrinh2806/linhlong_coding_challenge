namespace Pharma.Identity.Application.Common.Abstractions;

public interface ITotpService
{
    /// <summary>
    /// Generates a new 256-bit secret key encoded in Base32
    /// </summary>
    string GenerateSecretKey();

    /// <summary>
    /// Generates QR code URL following RFC 6238 format
    /// </summary>
    string GenerateQrCodeUrl(string userEmail, string secretKey);

    /// <summary>
    /// Generates current TOTP code from secret key
    /// </summary>
    string GenerateTotp(string secretKey);

    /// <summary>
    /// Validates TOTP code with time window tolerance (±30 seconds)
    /// </summary>
    bool ValidateTotp(string secretKey, string totpCode);

    /// <summary>
    /// Generates backup recovery codes
    /// </summary>
    List<string> GenerateBackupCodes();

    /// <summary>
    /// Encrypts secret key for secure storage
    /// </summary>
    Task<string> EncryptSecretKeyAsync(string secretKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts secret key from storage
    /// </summary>
    Task<string> DecryptSecretKeyAsync(string encryptedSecretKey, CancellationToken cancellationToken = default);
}