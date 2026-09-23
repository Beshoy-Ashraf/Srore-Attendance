namespace API.Controllers;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Authentication.Commands.Login;
using Application.Authentication.Commands.RefreshToken;
using Application.Authentication.Commands.Logout;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
        => Ok(await mediator.Send(command));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        => Ok(await mediator.Send(command));

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await mediator.Send(new LogoutCommand(userId, request.RefreshToken));
        return NoContent();
    }
}

public record LogoutRequest(string RefreshToken);
