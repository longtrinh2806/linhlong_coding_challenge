using System.Security.Cryptography;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Infrastructure.Configurations;

namespace Pharma.Identity.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(EncryptionConfiguration encryptionConfiguration)
    {
        _key = Convert.FromBase64String(encryptionConfiguration.Key);
        _iv = Convert.FromBase64String(encryptionConfiguration.Iv);

        // Validate key and IV sizes for AES-256
        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Encryption key must be 32 bytes (256 bits) for AES-256");
        }

        if (_iv.Length != 16)
        {
            throw new InvalidOperationException("Encryption IV must be 16 bytes (128 bits) for AES");
        }
    }

    public async Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        await using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        await using var swEncrypt = new StreamWriter(csEncrypt);

        await swEncrypt.WriteAsync(plainText);
        await swEncrypt.FlushAsync(cancellationToken);
        await csEncrypt.FlushFinalBlockAsync(cancellationToken);

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public async Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));
        }

        var cipherBytes = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(cipherBytes);
        await using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return await srDecrypt.ReadToEndAsync(cancellationToken);
    }
}