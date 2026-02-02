using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.OperationResult;
using Pharma.Identity.Application.Features.Authentication.DTOs;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Application.Features.Authentication.Commands;

public static class Login
{
    public record LoginCommand(string Email, string Password) : IRequest<OperationResult<LoginResponse>>;

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
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
                .WithMessage("Password is required");
        }
    }

    internal sealed class LoginHandler(
        ILogger<LoginHandler> logger,
        ICachingService cachingService,
        IHasher hasher,
        IJwtTokenService jwtTokenService,
        IGenericRepository<User> userRepository,
        IJwtTokenConfiguration jwtTokenConfiguration
    ) : IRequestHandler<LoginCommand, OperationResult<LoginResponse>>
    {
        private const int MaxFailedAttempts = 5;
        private const int LockoutDurationMinutes = 30;

        public async Task<OperationResult<LoginResponse>> Handle(LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetFirstOrDefaultAsync(
                user => user.Email == request.Email,
                include: user => user.Include(u => u.Role),
                cancellationToken: cancellationToken
            );

            if (user is null)
            {
                return OperationResult.BadRequest("Invalid email or password");
            }

            if (user is { IsAccountLocked: true, LockedUntil: not null } && user.LockedUntil > DateTime.UtcNow)
            {
                var remainingMinutes = Math.Ceiling((user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes);
                return OperationResult.BadRequest($"Account is locked. Try again in {remainingMinutes} minutes.");
            }

            var passwordValidationResult = ValidatePasswordAsync(user.Password, request.Password);
            if (passwordValidationResult != null)
            {
                await HandleFailedLoginAttemptAsync(user, cancellationToken);
                
                return passwordValidationResult;
            }

            await ResetFailedAttemptsAsync(user, cancellationToken);

            var tokens = await GenerateTokensAsync(user, cancellationToken);

            logger.LogInformation("Successful login for user {Email}", user.Email);

            return OperationResult.Ok(new LoginResponse(
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                UserRole: user.Role.Name)
            );
        }

        #region Private Methods

        private OperationResult<LoginResponse>? ValidatePasswordAsync(string hashPassword, string password)
        {
            if (hasher.IsValueValid(password, hashPassword))
            {
                return null;
            }

            return OperationResult.BadRequest("Invalid email or password");
        }

        private async Task HandleFailedLoginAttemptAsync(User user, CancellationToken cancellationToken)
        {
            user.FailedLoginAttempts++;
            user.LastFailedLoginAt = DateTime.UtcNow;

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.IsAccountLocked = true;
                user.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);

                logger.LogWarning("Account locked for user {Email} after {Attempts} failed attempts",
                    user.Email, MaxFailedAttempts);
            }

            await userRepository.UpdateAsync(user, cancellationToken);
        }

        private async Task ResetFailedAttemptsAsync(User user, CancellationToken cancellationToken)
        {
            if (user.FailedLoginAttempts > 0 || user.IsAccountLocked)
            {
                user.FailedLoginAttempts = 0;
                user.LastFailedLoginAt = null;
                user.IsAccountLocked = false;
                user.LockedUntil = null;

                await userRepository.UpdateAsync(user, cancellationToken);
            }
        }

        private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user,
            CancellationToken cancellationToken)
        {
            var refreshTokenCacheKey = Constant.CacheKeys.RefreshToken(user.UserId);

            var refreshToken = jwtTokenService.GenerateRefreshToken(user.UserId, user.Email);
            var accessToken = jwtTokenService.GenerateAccessToken(user.UserId, user.Email);

            await cachingService.SetAsync(
                key: refreshTokenCacheKey,
                value: refreshToken,
                expiration: TimeSpan.FromDays(jwtTokenConfiguration.RefreshTokenExpirationDays),
                cancellationToken: cancellationToken
            );

            return (accessToken, refreshToken);
        }

        #endregion
    }
}