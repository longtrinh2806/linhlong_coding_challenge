using FluentValidation;
using MassTransit;
using MediatR;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.OperationResult;
using Pharma.Identity.Application.Features.Authentication.Messages;

namespace Pharma.Identity.Application.Features.Authentication.Commands;

public static class ResendEmailOtp
{
    public record ResendEmailOtpCommand(string Email) : IRequest<OperationResult>;

    internal sealed class ResendEmailOtpCommandValidator : AbstractValidator<ResendEmailOtpCommand>
    {
        public ResendEmailOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");
        }
    }

    internal sealed class ResendEmailOtpCommandHandler(
        IEmailOtpService emailOtpService,
        IPublishEndpoint publishEndpoint
    ) : IRequestHandler<ResendEmailOtpCommand, OperationResult>
    {
        public async Task<OperationResult> Handle(ResendEmailOtpCommand request, CancellationToken cancellationToken)
        {
            var otp = await emailOtpService.GenerateEmailOtpAsync(
                email: request.Email,
                expiration: TimeSpan.FromMinutes(Constant.CacheExpiration.PendingUserMinutes),
                cancellationToken: cancellationToken
            );

            await publishEndpoint.Publish(
                new IdentityUserRegisteredEvent(request.Email, otp),
                context => { context.TimeToLive = TimeSpan.FromMinutes(Constant.CacheExpiration.EmailOtpMinutes); },
                cancellationToken
            );

            return OperationResult.Ok();
        }
    }
}