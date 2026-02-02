using Pharma.Identity.Application.Common.Abstractions;

namespace Pharma.Identity.Infrastructure.Services;

public class Hasher : IHasher
{
    public string HashValue(string value)
    {
        return BCrypt.Net.BCrypt.HashPassword(value);
    }

    public bool IsValueValid(string value, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(value, hash);
    }
}