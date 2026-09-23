using API.Contracts.Schedules;
using Application.Common.Models;
using Application.Schedules.Commands.ApproveSchedule;
using Application.Schedules.Commands.CreateSchedule;
using Application.Schedules.Commands.DeleteSchedule;
using Application.Schedules.Commands.RejectSchedule;
using Application.Schedules.Commands.UpdateSchedule;
using Application.Schedules.Dtos;
using Application.Schedules.Queries.GetPendingSchedules;
using Application.Schedules.Queries.GetScheduleApprovalAlerts;
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

      /// <summary>Store Manager creates a schedule for a staff member. Starts as Pending.</summary>
      [HttpPost]
      [Authorize(Roles = "StoreManager,Admin")]
      public async Task<ActionResult<Guid>> Create(CreateScheduleRequest request, CancellationToken cancellationToken)
      {
            var command = new CreateScheduleCommand(
                request.StaffId, request.Date, request.ShiftType, request.StartTime, request.EndTime);

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
            await _mediator.Send(new ApproveScheduleCommand(id), cancellationToken);
            return NoContent();
      }

      /// <summary>Area Manager rejects a pending schedule. The schedule is then soft-deleted but stays
      /// visible as rejected history (GET /api/schedules?status=Rejected).</summary>
      [HttpPost("{id:guid}/reject")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Reject(Guid id, RejectScheduleRequest request, CancellationToken cancellationToken)
      {
            await _mediator.Send(new RejectScheduleCommand(id, request.Reason), cancellationToken);
            return NoContent();
      }

      /// <summary>Withdraw a schedule you created (Pending only; an Approved one must go through reject).</summary>
      [HttpDelete("{id:guid}")]
      [Authorize(Roles = "StoreManager,Admin")]
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

      /// <summary>List schedules filtered by staff/store/date range/status. status=Rejected returns rejected history.</summary>
      [HttpGet]
      public async Task<ActionResult<PagedResult<ScheduleDto>>> GetAll(
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
      public async Task<ActionResult<PagedResult<ScheduleDto>>> GetPending(
          [FromQuery] Guid? storeId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var schedules = await _mediator.Send(new GetPendingSchedulesQuery(storeId, page, pageSize), cancellationToken);
            return Ok(schedules);
      }

      /// <summary>Stores/months whose schedules haven't been approved yet, at or near month start — the
      /// "approve or reject before the month arrives" alert.</summary>
      [HttpGet("alerts")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<ActionResult<IReadOnlyList<ScheduleApprovalAlertDto>>> GetAlerts(CancellationToken cancellationToken)
      {
            var alerts = await _mediator.Send(new GetScheduleApprovalAlertsQuery(), cancellationToken);
            return Ok(alerts);
      }
}
