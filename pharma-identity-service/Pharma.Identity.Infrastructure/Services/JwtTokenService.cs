using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pharma.Identity.Application.Common.Abstractions;

namespace Pharma.Identity.Infrastructure.Services;

public class JwtTokenService(IJwtTokenConfiguration jwtTokenConfiguration) : IJwtTokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public string GenerateAccessToken(Ulid userId, string email)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfiguration.SecretKey));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("tokenType", "access")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtTokenConfiguration.AccessTokenExpirationMinutes),
            Issuer = jwtTokenConfiguration.Issuer,
            Audience = jwtTokenConfiguration.Audience,
            SigningCredentials = signingCredentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);

        return _tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(Ulid userId, string email, TimeSpan? expiration = null)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfiguration.SecretKey));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("tokenType", "refresh")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration.HasValue
                ? DateTime.UtcNow.Add(expiration.Value)
                : DateTime.UtcNow.AddDays(jwtTokenConfiguration.RefreshTokenExpirationDays),
            Issuer = jwtTokenConfiguration.Issuer,
            Audience = jwtTokenConfiguration.Audience,
            SigningCredentials = signingCredentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);

        return _tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateRefreshToken(string refreshToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtTokenConfiguration.Issuer,
            ValidAudience = jwtTokenConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfiguration.SecretKey)),
            ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
        };

        try
        {
            var principal =
                _tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out var validatedToken);

            var tokenTypeClaim = principal.FindFirst("tokenType")?.Value;
            if (tokenTypeClaim != "refresh")
            {
                return null;
            }

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}