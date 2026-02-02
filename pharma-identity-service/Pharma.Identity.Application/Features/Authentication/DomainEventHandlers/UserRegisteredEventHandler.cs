using MassTransit;
using MediatR;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Features.Authentication.DomainEvents;
using Pharma.Identity.Application.Features.Authentication.Messages;

namespace Pharma.Identity.Application.Features.Authentication.DomainEventHandlers;

public class UserRegisteredEventHandler(
    IEmailOtpService emailOtpService,
    ICachingService cachingService,
    IPublishEndpoint publishEndpoint
) : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var otp = await emailOtpService.GenerateEmailOtpAsync(
            notification.User.Email,
            TimeSpan.FromMinutes(Constant.CacheExpiration.EmailOtpMinutes),
            cancellationToken
        );

        var pendingUserCacheKey = Constant.CacheKeys.PendingUser(notification.User.Email);

        await cachingService.SetAsync(
            key: pendingUserCacheKey,
            value: notification.User,
            expiration: TimeSpan.FromMinutes(Constant.CacheExpiration.PendingUserMinutes),
            cancellationToken: cancellationToken
        );

        await publishEndpoint.Publish(
            new IdentityUserRegisteredEvent(notification.User.Email, otp),
            context => { context.TimeToLive = TimeSpan.FromMinutes(Constant.CacheExpiration.EmailOtpMinutes); },
            cancellationToken
        );
    }
}