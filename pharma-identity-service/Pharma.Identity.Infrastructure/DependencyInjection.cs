using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Infrastructure.Configurations;
using Pharma.Identity.Infrastructure.Persistence;
using Pharma.Identity.Infrastructure.Persistence.Interceptors;
using Pharma.Identity.Infrastructure.Repositories;
using Pharma.Identity.Infrastructure.Services;
using Serilog;

namespace Pharma.Identity.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Serilog Configuration

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        #endregion
        
        #region Caching Configuration

        var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")!;
        var defaultCacheExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("DEFAULT_CACHE_EXPIRATION_MINUTES"), out var result) ? result : 15;

        var cacheConfiguration = new CacheConfiguration
        {
            DefaultExpiration = TimeSpan.FromMinutes(defaultCacheExpirationMinutes)
        };
        services.AddSingleton(cacheConfiguration);
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        #endregion

        #region MassTransit Configuration

        var rabbitMqConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")!;

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                var uri = new Uri(rabbitMqConnectionString);

                config.Host(uri.Host, h =>
                {
                    if (string.IsNullOrEmpty(uri.UserInfo)) return;

                    var userInfo = uri.UserInfo.Split(':');
                    h.Username(userInfo[0]);

                    if (userInfo.Length > 1)
                    {
                        h.Password(userInfo[1]);
                    }
                });

                config.ConfigureEndpoints(context);
            });
        });

        #endregion

        #region Jwt Configuration
        
        var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!;
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
        var jwtAccessTokenExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"), out var accessTokenExp) ? accessTokenExp : 60;
        var jwtRefreshTokenExpirationDays = int.TryParse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_DAYS"), out var refreshTokenExp) ? refreshTokenExp : 7;
        
        var jwtConfiguration = new JwtTokenConfiguration
        {
            Audience = jwtAudience,
            Issuer = jwtIssuer,
            SecretKey = jwtSecretKey,
            AccessTokenExpirationMinutes = jwtAccessTokenExpirationMinutes,
            RefreshTokenExpirationDays = jwtRefreshTokenExpirationDays
        };

        services.AddSingleton<IJwtTokenConfiguration>(jwtConfiguration);
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfiguration.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfiguration.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
            
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var tokenTypeClaim = context.Principal?.FindFirst("tokenType")?.Value;

                    if (tokenTypeClaim != "access")
                    {
                        context.Fail("Invalid token type. Only access tokens are allowed.");
                    }

                    return Task.CompletedTask;
                }
            };
        });

        #endregion

        #region DbContext Configuration

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;
        var readonlyConnectionString = Environment.GetEnvironmentVariable("READONLY_DB_CONNECTION_STRING")!;

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });
        
        services.AddDbContext<ReadOnlyDbContext>((options) =>
        {
            options
                .UseNpgsql(readonlyConnectionString)
                .UseSnakeCaseNamingConvention();
        });

        #endregion

        #region Encryption Configuration

        var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")!;
        var encryptionIv = Environment.GetEnvironmentVariable("ENCRYPTION_IV")!;

        var encryptionConfiguration = new EncryptionConfiguration
        {
            Key = encryptionKey,
            Iv = encryptionIv
        };

        services.AddSingleton(encryptionConfiguration);

        #endregion

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ICachingService, RedisCachingService>();
        services.AddScoped<IHasher, Hasher>();
        services.AddScoped<IEmailOtpService, EmailOtpService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ITotpService, TotpService>();

        services.AddHttpContextAccessor();
    }
}