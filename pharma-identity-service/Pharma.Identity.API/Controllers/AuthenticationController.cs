using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pharma.Identity.API.Extensions;
using Pharma.Identity.Application.Features.Authentication.Commands;
using Pharma.Identity.Application.Features.Authentication.DTOs;

namespace Pharma.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new Register.RegisterCommand(request.Email, request.Password, request.ConfirmPassword),
            cancellationToken
        );

        return result.ToResult();
    }

    [HttpPost("login")]
    public async Task<IResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new Login.LoginCommand(request.Email, request.Password), cancellationToken);

        return result.ToResult();
    }

    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RefreshToken.RefreshTokenCommand(request.RefreshToken),
            cancellationToken
        );

        return result.ToResult();
    }
}