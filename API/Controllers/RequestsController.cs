using System.Security.Claims;
using API.Contracts.Requests;
using Application.Requests.Commands.ApproveRequest;
using Application.Requests.Commands.CreateRequest;
using Application.Requests.Commands.RejectRequest;
using Application.Requests.Dtos;
using Application.Requests.Queries.GetPendingRequests;
using Application.Requests.Queries.GetRequestById;
using Application.Requests.Queries.GetRequests;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestsController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      // RequestedById / ApprovedByAreaManagerId always come from the token, never the body.
      private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("No user id claim found on the current request."));

      /// <summary>Staff (or their Store Manager, on their behalf) submits an exception, replacement,
      /// sick leave, annual, or official-holiday request. Starts as Pending.</summary>
      [HttpPost]
      public async Task<ActionResult<Guid>> Create(CreateRequestRequest request, CancellationToken cancellationToken)
      {
            var command = new CreateRequestCommand(
                request.StaffId, CurrentUserId, request.Type, request.DateFrom, request.DateTo, request.Reason);

            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      /// <summary>Area Manager approves a pending request. Annual/sick-leave/official-holiday
      /// requests are reflected onto the staff member's Schedule automatically.</summary>
      [HttpPost("{id:guid}/approve")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new ApproveRequestCommand(id, CurrentUserId), cancellationToken);
            return NoContent();
      }

      /// <summary>Area Manager rejects a pending request.</summary>
      [HttpPost("{id:guid}/reject")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new RejectRequestCommand(id, CurrentUserId), cancellationToken);
            return NoContent();
      }

      /// <summary>Get a single request by id.</summary>
      [HttpGet("{id:guid}")]
      public async Task<ActionResult<RequestDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var result = await _mediator.Send(new GetRequestByIdQuery(id), cancellationToken);
            return Ok(result);
      }

      /// <summary>List requests filtered by staff/type/status.</summary>
      [HttpGet]
      public async Task<ActionResult<IEnumerable<RequestDto>>> GetAll(
          [FromQuery] Guid? staffId,
          [FromQuery] RequestType? type,
          [FromQuery] RequestStatus? status,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var query = new GetRequestsQuery(staffId, type, status, page, pageSize);
            var results = await _mediator.Send(query, cancellationToken);
            return Ok(results);
      }

      /// <summary>Area Manager's queue of requests awaiting their approval.</summary>
      [HttpGet("pending")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<ActionResult<IEnumerable<RequestDto>>> GetPending(CancellationToken cancellationToken)
      {
            var results = await _mediator.Send(new GetPendingRequestsQuery(CurrentUserId), cancellationToken);
            return Ok(results);
      }
}