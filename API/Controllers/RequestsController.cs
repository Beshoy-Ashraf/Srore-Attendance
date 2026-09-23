using API.Contracts.Requests;
using Application.Common.Models;
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

      /// <summary>Staff (or their Store Manager, on their behalf) submits an exception, replacement,
      /// sick leave, annual, or official-holiday request. Starts as Pending.</summary>
      [HttpPost]
      public async Task<ActionResult<Guid>> Create(CreateRequestRequest request, CancellationToken cancellationToken)
      {
            var command = new CreateRequestCommand(
                request.StaffId, request.Type, request.DateFrom, request.DateTo, request.Reason);

            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      /// <summary>Area Manager approves a pending request. Annual/sick-leave/official-holiday
      /// requests are reflected onto the staff member's Schedule automatically.</summary>
      [HttpPost("{id:guid}/approve")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new ApproveRequestCommand(id), cancellationToken);
            return NoContent();
      }

      /// <summary>Area Manager rejects a pending request. The request is then soft-deleted but stays
      /// visible as rejected history (GET /api/requests?status=Rejected).</summary>
      [HttpPost("{id:guid}/reject")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Reject(Guid id, RejectRequestRequest request, CancellationToken cancellationToken)
      {
            await _mediator.Send(new RejectRequestCommand(id, request.Reason), cancellationToken);
            return NoContent();
      }

      /// <summary>Get a single request by id.</summary>
      [HttpGet("{id:guid}")]
      public async Task<ActionResult<RequestDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var result = await _mediator.Send(new GetRequestByIdQuery(id), cancellationToken);
            return Ok(result);
      }

      /// <summary>List requests filtered by staff/store/type/status. status=Rejected returns rejected history.</summary>
      [HttpGet]
      public async Task<ActionResult<PagedResult<RequestDto>>> GetAll(
          [FromQuery] Guid? staffId,
          [FromQuery] Guid? storeId,
          [FromQuery] RequestType? type,
          [FromQuery] RequestStatus? status,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var query = new GetRequestsQuery(staffId, storeId, type, status, page, pageSize);
            var results = await _mediator.Send(query, cancellationToken);
            return Ok(results);
      }

      /// <summary>Area Manager's queue of requests awaiting their approval.</summary>
      [HttpGet("pending")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<ActionResult<PagedResult<RequestDto>>> GetPending(
          [FromQuery] Guid? storeId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var results = await _mediator.Send(new GetPendingRequestsQuery(storeId, page, pageSize), cancellationToken);
            return Ok(results);
      }
}
