namespace Pharma.Identity.Infrastructure.Configurations;

public class EncryptionConfiguration
{
    public required string Key { get; set; }

    public required string Iv { get; set; }
}