namespace Pharma.Identity.Application.Common.Abstractions;

public interface IEncryptionService
{
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);
    Task<string> DecryptAsync(string encryptedText, CancellationToken cancellationToken = default);
}