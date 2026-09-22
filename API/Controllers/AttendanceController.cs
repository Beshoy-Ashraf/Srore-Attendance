using System.Security.Claims;
using API.Contracts.Attendance;
using Application.Attendance.Commands.CheckIn;
using Application.Attendance.Commands.CheckOut;
using Application.Attendance.Commands.ManualAttendance;
using Application.Attendance.Dtos;
using Application.Attendance.Queries.GetAttendanceById;
using Application.Attendance.Queries.GetAttendances;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("No user id claim found on the current request."));

      [HttpPost("check-in")]
      public async Task<ActionResult<CheckInResponseDto>> CheckIn(CheckInRequest request, CancellationToken cancellationToken)
      {
            var command = new CheckInCommand(CurrentUserId, request.RouterMac, request.DeviceMac);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
      }

      [HttpPost("check-out")]
      public async Task<ActionResult<CheckOutResponseDto>> CheckOut(CheckOutRequest request, CancellationToken cancellationToken)
      {
            var command = new CheckOutCommand(CurrentUserId, request.RouterMac, request.DeviceMac);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
      }

      [HttpPost("manual")]
      [Authorize(Roles = "StoreManager,AreaManager,Admin")]
      public async Task<ActionResult<Guid>> ManualEntry(ManualAttendanceRequest request, CancellationToken cancellationToken)
      {
            var command = new ManualAttendanceCommand(
                request.StaffId, CurrentUserId, request.Date, request.CheckInTime, request.CheckOutTime, request.Notes);

            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      [HttpGet("{id:guid}")]
      public async Task<ActionResult<AttendanceDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var attendance = await _mediator.Send(new GetAttendanceByIdQuery(id), cancellationToken);
            return Ok(attendance);
      }

      [HttpGet]
      [Authorize(Roles = "StoreManager,AreaManager,Admin")]
      public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAll(
          [FromQuery] Guid? staffId,
          [FromQuery] Guid? storeId,
          [FromQuery] DateTime? from,
          [FromQuery] DateTime? to,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var query = new GetAttendancesQuery(staffId, storeId, from, to, page, pageSize);
            var attendances = await _mediator.Send(query, cancellationToken);
            return Ok(attendances);
      }

      [HttpGet("me")]
      public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetMine(
          [FromQuery] DateTime? from,
          [FromQuery] DateTime? to,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var query = new GetAttendancesQuery(CurrentUserId, null, from, to, page, pageSize);
            var attendances = await _mediator.Send(query, cancellationToken);
            return Ok(attendances);
      }
}