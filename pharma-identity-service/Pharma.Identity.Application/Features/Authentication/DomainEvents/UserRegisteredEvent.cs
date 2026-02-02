using MediatR;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Application.Features.Authentication.DomainEvents;

public record UserRegisteredEvent(User User) : INotification;