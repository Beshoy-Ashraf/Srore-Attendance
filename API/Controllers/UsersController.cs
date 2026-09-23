using Application.Common.Models;
using Application.Users.Commands.ChangePassword;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.ResetPassword;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Queries.GetUserById;
using Application.Users.Queries.GetUsers;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      [HttpGet("{id:guid}")]
      public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
            return Ok(user);
      }

      [HttpGet]
      public async Task<ActionResult<PagedResult<UserDto>>> GetAll(
          [FromQuery] Guid? storeId,
          [FromQuery] UserRole? role,
          [FromQuery] string? search,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var users = await _mediator.Send(new GetUsersQuery(storeId, role, search, page, pageSize), cancellationToken);
            return Ok(users);
      }

      [HttpPost]
      [Authorize(Roles = "Admin,AreaManager")]
      public async Task<ActionResult<Guid>> Create(CreateUserCommand command, CancellationToken cancellationToken)
      {
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      [HttpPut("{id:guid}")]
      public async Task<IActionResult> Update(Guid id, UpdateUserCommand command, CancellationToken cancellationToken)
      {
            if (id != command.Id)
                  return BadRequest("Route id and body id must match.");

            await _mediator.Send(command, cancellationToken);
            return NoContent();
      }

      [HttpDelete("{id:guid}")]
      [Authorize(Roles = "Admin,AreaManager")]
      public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
            return NoContent();
      }

      [HttpPost("me/change-password")]
      public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
      {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
      }

      [HttpPost("{id:guid}/reset-password")]
      [Authorize(Roles = "Admin,AreaManager")]
      public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordBody body, CancellationToken cancellationToken)
      {
            await _mediator.Send(new ResetPasswordCommand(id, body.NewPassword), cancellationToken);
            return NoContent();
      }
}

public record ResetPasswordBody(string NewPassword);
