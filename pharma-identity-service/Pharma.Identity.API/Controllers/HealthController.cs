using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pharma.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthController
{
    [HttpGet]
    public IResult HealthCheck()
    {
        return Results.Ok("Pharma Identity Service is healthy.");
    }
}