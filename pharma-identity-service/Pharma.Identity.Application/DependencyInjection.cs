using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pharma.Identity.Application.Common.Behaviors;
using Pharma.Identity.Application.Features.Authentication.Commands;

namespace Pharma.Identity.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        #region MediatR Configuration

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        #endregion

        services.AddValidatorsFromAssemblyContaining<Register.RegisterCommandValidator>();
    }
}