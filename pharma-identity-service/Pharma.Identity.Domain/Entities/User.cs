using System.ComponentModel.DataAnnotations.Schema;
using Pharma.Identity.Domain.Abstractions;

namespace Pharma.Identity.Domain.Entities;

public class User : AuditableEntity
{
    public required Ulid UserId { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsTwoFactorEnabled { get; set; }
    
    public string? TotpSecretEncrypted { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? HashedBackupCodes { get; set; }
    
    public bool IsAccountLocked { get; set; }
}