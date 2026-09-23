using Application.Devices.Commands.ResetDevice;
using Application.Devices.Dtos;
using Application.Devices.Queries.GetDeviceByStaff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/staff/{staffId:guid}/device")]
[Authorize]
public class DevicesController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      /// <summary>The staff member's currently registered device, if any.</summary>
      [HttpGet]
      public async Task<ActionResult<DeviceDto?>> Get(Guid staffId, CancellationToken cancellationToken)
      {
            var device = await _mediator.Send(new GetDeviceByStaffQuery(staffId), cancellationToken);
            return device is null ? NotFound() : Ok(device);
      }

      /// <summary>Store Manager/Area Manager/Admin resets a staff member's registered device — e.g. they got a new PC.
      /// The next successful check-in registers whatever machine they use.</summary>
      [HttpPost("reset")]
      [Authorize(Roles = "StoreManager,AreaManager,Admin")]
      public async Task<IActionResult> Reset(Guid staffId, CancellationToken cancellationToken)
      {
            await _mediator.Send(new ResetDeviceCommand(staffId), cancellationToken);
            return NoContent();
      }
}
