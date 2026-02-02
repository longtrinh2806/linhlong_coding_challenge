using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.OperationResult;
using Pharma.Identity.Application.Features.Authentication.DTOs;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Application.Features.Authentication.Commands;

public static class Login
{
    public record LoginCommand(string Email, string Password) : IRequest<OperationResult<LoginResponse>>;

    internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters");
        }
    }

    internal sealed class LoginHandler(
        ILogger<LoginHandler> logger,
        ICachingService cachingService,
        IHasher hasher,
        IJwtTokenService jwtTokenService,
        IReadOnlyRepository<User> userRepository,
        IJwtTokenConfiguration jwtTokenConfiguration
    ) : IRequestHandler<LoginCommand, OperationResult<LoginResponse>>
    {
        public async Task<OperationResult<LoginResponse>> Handle(LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetFirstOrDefaultAsync(
                user => user.Email == request.Email,
                cancellationToken: cancellationToken
            );

            if (user is null || !hasher.IsValueValid(request.Password, user.Password))
            {
                return OperationResult.BadRequest("Invalid email or password");
            }

            if (user.IsAccountLocked)
            {
                return OperationResult.BadRequest("Account is locked");
            }

            var refreshTokenCacheKey = Constant.CacheKeys.RefreshToken(user.UserId);
            try
            {
                await cachingService.RemoveAsync(refreshTokenCacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to remove old refresh token for user {UserId}", user.UserId);
            }

            var refreshToken = jwtTokenService.GenerateRefreshToken(user.UserId, user.Email);
            var accessToken = jwtTokenService.GenerateAccessToken(user.UserId, user.Email);
            try
            {
                await cachingService.SetAsync(
                    key: refreshTokenCacheKey,
                    value: refreshToken,
                    expiration: TimeSpan.FromDays(jwtTokenConfiguration.RefreshTokenExpirationDays),
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return OperationResult.Ok(new LoginResponse(
                RequiresTwoFactor: false,
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                TwoFactorToken: null)
            );
        }
    }
}