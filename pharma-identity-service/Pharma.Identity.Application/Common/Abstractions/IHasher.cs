namespace Pharma.Identity.Application.Common.Abstractions;

public interface IHasher
{
    string HashValue(string value);

    bool IsValueValid(string value, string hash);
}