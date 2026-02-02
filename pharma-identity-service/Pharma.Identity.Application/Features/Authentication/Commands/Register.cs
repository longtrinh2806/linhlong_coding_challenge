using FluentValidation;
using MediatR;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.OperationResult;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Application.Features.Authentication.Commands;

public static class Register
{
    public record RegisterCommand(string Email, string Password, string ConfirmPassword) : IRequest<OperationResult>;

    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(12)
                .WithMessage("Password must be at least 12 characters")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]")
                .WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match");
        }
    }

    internal sealed class RegisterCommandHandler(
        IHasher hasher,
        IGenericRepository<User> userRepository
    ) : IRequestHandler<RegisterCommand, OperationResult>
    {
        public async Task<OperationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetFirstOrDefaultAsync(
                user => user.Email == request.Email,
                cancellationToken: cancellationToken
            );

            if (existingUser is not null)
            {
                return OperationResult.BadRequest("User with the given email already exists.");
            }

            var newUser = new User
            {
                UserId = Ulid.NewUlid(),
                Email = request.Email,
                Password = hasher.HashValue(request.Password),
                RoleId = Constant.RoleIds.Viewer
            };

            await userRepository.AddAsync(newUser, cancellationToken);

            return OperationResult.Ok();
        }
    }
}