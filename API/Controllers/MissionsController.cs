using API.Contracts.Missions;
using Application.Common.Models;
using Application.Missions.Commands.ApproveMission;
using Application.Missions.Commands.CreateMission;
using Application.Missions.Commands.RejectMission;
using Application.Missions.Dtos;
using Application.Missions.Queries.GetMissionById;
using Application.Missions.Queries.GetMissions;
using Application.Missions.Queries.GetPendingMissions;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MissionsController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      /// <summary>Submit a mission (required/official travel, e.g. a business trip). Starts as Pending.</summary>
      [HttpPost]
      public async Task<ActionResult<Guid>> Create(CreateMissionRequest request, CancellationToken cancellationToken)
      {
            var command = new CreateMissionCommand(request.StaffId, request.Reason, request.DateFrom, request.DateTo);
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      /// <summary>Area Manager approves a pending mission.</summary>
      [HttpPost("{id:guid}/approve")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
      {
            await _mediator.Send(new ApproveMissionCommand(id), cancellationToken);
            return NoContent();
      }

      /// <summary>Area Manager rejects a pending mission. The mission is then soft-deleted but stays
      /// visible as rejected history (GET /api/missions?status=Rejected).</summary>
      [HttpPost("{id:guid}/reject")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<IActionResult> Reject(Guid id, RejectMissionRequest request, CancellationToken cancellationToken)
      {
            await _mediator.Send(new RejectMissionCommand(id, request.Reason), cancellationToken);
            return NoContent();
      }

      /// <summary>Get a single mission by id.</summary>
      [HttpGet("{id:guid}")]
      public async Task<ActionResult<MissionDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var result = await _mediator.Send(new GetMissionByIdQuery(id), cancellationToken);
            return Ok(result);
      }

      /// <summary>List missions filtered by staff/store/status. status=Rejected returns rejected history.</summary>
      [HttpGet]
      public async Task<ActionResult<PagedResult<MissionDto>>> GetAll(
          [FromQuery] Guid? staffId,
          [FromQuery] Guid? storeId,
          [FromQuery] RequestStatus? status,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var results = await _mediator.Send(new GetMissionsQuery(staffId, storeId, status, page, pageSize), cancellationToken);
            return Ok(results);
      }

      /// <summary>Area Manager's queue of missions awaiting their approval.</summary>
      [HttpGet("pending")]
      [Authorize(Roles = "AreaManager,Admin")]
      public async Task<ActionResult<PagedResult<MissionDto>>> GetPending(
          [FromQuery] Guid? storeId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var results = await _mediator.Send(new GetPendingMissionsQuery(storeId, page, pageSize), cancellationToken);
            return Ok(results);
      }
}
