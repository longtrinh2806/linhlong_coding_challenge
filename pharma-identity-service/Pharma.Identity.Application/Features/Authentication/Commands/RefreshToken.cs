using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.OperationResult;
using Pharma.Identity.Application.Features.Authentication.DTOs;

namespace Pharma.Identity.Application.Features.Authentication.Commands;

public static class RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<OperationResult<LoginResponse>>;

    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required");
        }
    }

    internal sealed class RefreshTokenCommandHandler(
        ICachingService cachingService,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenCommandHandler> logger
    ) : IRequestHandler<RefreshTokenCommand, OperationResult<LoginResponse>>
    {
        public async Task<OperationResult<LoginResponse>> Handle(RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var claimsPrincipal = jwtTokenService.ValidateRefreshToken(request.RefreshToken);
            if (claimsPrincipal is null)
            {
                return OperationResult.Unauthorized("Invalid refresh token");
            }

            var claimsValidationResult = ExtractAndValidateClaims(claimsPrincipal);
            if (!claimsValidationResult.IsSuccess)
            {
                return claimsValidationResult.Error!;
            }

            var (userId, email) = claimsValidationResult.Value;

            var cacheValidationResult = await ValidateTokenInCache(userId, request.RefreshToken, cancellationToken);
            if (!cacheValidationResult.IsSuccess)
            {
                return cacheValidationResult.Error!;
            }

            var ttlResult = CalculateRemainingTtl(claimsPrincipal, userId);
            if (!ttlResult.IsSuccess)
            {
                return ttlResult.Error!;
            }

            var remainingTtl = ttlResult.Value;

            var newAccessToken = jwtTokenService.GenerateAccessToken(userId, email);
            var newRefreshToken = jwtTokenService.GenerateRefreshToken(
                userId: userId,
                email: email,
                expiration: remainingTtl
            );

            var storeResult = await StoreNewRefreshToken(userId, newRefreshToken, remainingTtl, cancellationToken);
            if (!storeResult.IsSuccess)
            {
                return storeResult.Error!;
            }

            logger.LogInformation(
                "Successfully refreshed tokens for user {UserId} with remaining TTL {RemainingMinutes:F2} minutes",
                userId, remainingTtl.TotalMinutes);

            return OperationResult.Ok(new LoginResponse(
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken,
                UserRole: null
            ));
        }

        #region Private Methods

        private static (
            bool IsSuccess,
            (Ulid UserId, string Email) Value,
            OperationResult<LoginResponse>? Error) ExtractAndValidateClaims(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "userId");
            var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (userIdClaim is null || !Ulid.TryParse(userIdClaim.Value, out var userId) ||
                string.IsNullOrEmpty(emailClaim))
            {
                return (false, default, OperationResult.Unauthorized("Invalid token claims"));
            }

            return (true, (userId, emailClaim), null);
        }

        private async Task<(bool IsSuccess, OperationResult<LoginResponse>? Error)> ValidateTokenInCache(
            Ulid userId,
            string requestToken,
            CancellationToken cancellationToken)
        {
            var refreshTokenCacheKey = Constant.CacheKeys.RefreshToken(userId);

            try
            {
                var cachedRefreshToken = await cachingService.GetAsync<string>(refreshTokenCacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedRefreshToken) && cachedRefreshToken == requestToken)
                {
                    return (true, null);
                }

                logger.LogWarning("Refresh token mismatch or not found for user {UserId}", userId);
                return (false, OperationResult.Unauthorized("Invalid or expired refresh token"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to retrieve refresh token from cache for user {UserId}", userId);
                return (false, OperationResult.BadRequest("Failed to validate refresh token"));
            }
        }

        private (bool IsSuccess, TimeSpan Value, OperationResult<LoginResponse>? Error) CalculateRemainingTtl(
            ClaimsPrincipal principal,
            Ulid userId)
        {
            var expirationClaim = principal.Claims.FirstOrDefault(c => c.Type == "exp");
            if (expirationClaim is null || !long.TryParse(expirationClaim.Value, out var expirationUnix))
            {
                return (false, TimeSpan.Zero, OperationResult.Unauthorized("Invalid token expiration"));
            }

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationUnix);
            var remainingTtl = expirationTime - DateTimeOffset.UtcNow;

            if (remainingTtl > TimeSpan.Zero)
            {
                return (true, remainingTtl, null);
            }

            logger.LogWarning("Refresh token expired for user {UserId}", userId);
            return (false, TimeSpan.Zero, OperationResult.Unauthorized("Refresh token expired"));
        }

        private async Task<(bool IsSuccess, OperationResult<LoginResponse>? Error)> StoreNewRefreshToken(
            Ulid userId,
            string newRefreshToken,
            TimeSpan remainingTtl,
            CancellationToken cancellationToken)
        {
            var refreshTokenCacheKey = Constant.CacheKeys.RefreshToken(userId);

            try
            {
                await cachingService.SetAsync(
                    key: refreshTokenCacheKey,
                    value: newRefreshToken,
                    expiration: remainingTtl,
                    cancellationToken: cancellationToken
                );
                return (true, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to store new refresh token for user {UserId}", userId);
                return (false, OperationResult.BadRequest("Failed to refresh token. Please login again."));
            }
        }

        #endregion
    }
}