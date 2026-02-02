using FluentValidation;
using MediatR;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.OperationResult;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Application.Features.Authentication.Commands;

public static class ValidateEmailOtp
{
    public record ValidateEmailOtpCommand(string Email, string Otp) : IRequest<OperationResult>;

    internal sealed class ValidateEmailOtpValidator : AbstractValidator<ValidateEmailOtpCommand>
    {
        public ValidateEmailOtpValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(x => x.Otp)
                .NotEmpty()
                .WithMessage("Otp is required")
                .Length(6)
                .WithMessage("Otp must be 6 characters");
        }
    }

    internal sealed class ValidateEmailOtpCommandHandler(
        IEmailOtpService emailOtpService,
        ICachingService cachingService,
        IGenericRepository<User> userRepository
    ) : IRequestHandler<ValidateEmailOtpCommand, OperationResult>
    {
        public async Task<OperationResult> Handle(ValidateEmailOtpCommand request, CancellationToken cancellationToken)
        {
            var isValidOtp = await emailOtpService.VerifyEmailOtpAsync(email: request.Email, otp: request.Otp, cancellationToken);

            if (!isValidOtp)
            {
                return OperationResult.BadRequest("Invalid or expired OTP.");
            }

            var pendingUserCacheKey = Constant.CacheKeys.PendingUser(request.Email);

            var pendingUser = await cachingService.GetAsync<User>(pendingUserCacheKey, cancellationToken);

            if (pendingUser is null)
            {
                return OperationResult.BadRequest("No pending registration found for the provided email.");
            }

            await userRepository.AddAsync(pendingUser, cancellationToken);
            await cachingService.RemoveAsync(pendingUserCacheKey, cancellationToken);

            return OperationResult.Ok();
        }
    }
}