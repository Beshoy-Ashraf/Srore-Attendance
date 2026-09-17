using System.Security.Claims;
using API.Contracts.Schedules;
using Application.Schedules.Commands.ApproveSchedule;
using Application.Schedules.Commands.CreateSchedule;
using Application.Schedules.Commands.DeleteSchedule;
using Application.Schedules.Commands.RejectSchedule;
using Application.Schedules.Commands.UpdateSchedule;
using Application.Schedules.Dtos;
using Application.Schedules.Queries.GetPendingSchedules;
using Application.Schedules.Queries.GetScheduleById;
using Application.Schedules.Queries.GetSchedules;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      // CreatedByStoreManagerId / ApprovedByAreaManagerId always come from the token,
      // never from the request body — otherwise anyone could submit or approve as someone else.
      private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("No user id claim found on the current request."));

      /// <summary>Store Manager creates a schedule for a staff member. Starts as Pending.</summary>
      [HttpPost]
      [Authorize(Roles = "StoreManager,Admin")]
      public async Task<ActionResult<Guid>> Create(CreateScheduleRequest request, CancellationToken cancellationToken)
      {
            var command = new CreateScheduleCommand(
                request.StaffId, CurrentUserId, request.Date, request.ShiftType, request.StartTime, request.EndTime);

            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      /// <summary>Store Manager edits a schedule. Any edit resets it to Pending for re-approval.</summary>
      [HttpPut("{id:guid}")]
      [Authorize(Roles = "StoreManager,Admin")]
      public async Task<IActionResult> Update(Guid id, UpdateScheduleRequest request, CancellationToken cancellationToken)
      {
            var command = new UpdateScheduleCommand(id, request.ShiftType, request.StartTime, request.EndTime);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
      }

      /// <summary>Area Manager approves a pending schedule.</summary>
      [HttpPost("{id:guid}/approve")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new ApproveScheduleCommand(id, CurrentUserId), cancellationToken);
            return NoContent();
      }

      /// <summary>Area Manager rejects a pending schedule.</summary>
      [HttpPost("{id:guid}/reject")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new RejectScheduleCommand(id, CurrentUserId), cancellationToken);
            return NoContent();
      }

      /// <summary>Delete a schedule.</summary>
      [HttpDelete("{id:guid}")]
      [Authorize(Roles = "StoreManager,AreaManager,Admin")]
      public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new DeleteScheduleCommand(id), cancellationToken);
            return NoContent();
      }

      /// <summary>Get a single schedule by id.</summary>
      [HttpGet("{id:guid}")]
      public async Task<ActionResult<ScheduleDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var schedule = await _mediator.Send(new GetScheduleByIdQuery(id), cancellationToken);
            return Ok(schedule);
      }

      /// <summary>List schedules filtered by staff/store/date range/status.</summary>
      [HttpGet]
      public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll(
          [FromQuery] Guid? staffId,
          [FromQuery] Guid? storeId,
          [FromQuery] DateOnly? from,
          [FromQuery] DateOnly? to,
          [FromQuery] ScheduleStatus? status,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var query = new GetSchedulesQuery(staffId, storeId, from, to, status, page, pageSize);
            var schedules = await _mediator.Send(query, cancellationToken);
            return Ok(schedules);
      }

      /// <summary>Area Manager's queue of schedules awaiting their approval.</summary>
      [HttpGet("pending")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetPending(CancellationToken cancellationToken)
      {
            var schedules = await _mediator.Send(new GetPendingSchedulesQuery(CurrentUserId), cancellationToken);
            return Ok(schedules);
      }
}