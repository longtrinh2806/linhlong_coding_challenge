namespace Pharma.Identity.Application.Common.Abstractions;

public interface ICurrentUser
{
    Ulid UserId { get; }

    string? UserEmail { get; }
}