using Application.AttendanceSettings.Commands.UpsertAttendanceSettings;
using Application.AttendanceSettings.Dtos;
using Application.AttendanceSettings.Queries.GetAttendanceSettingsByStore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/stores/{storeId:guid}/attendance-settings")]
[Authorize]
public class AttendanceSettingsController : ControllerBase
{
      private readonly ISender _mediator;

      public AttendanceSettingsController(ISender mediator)
      {
            _mediator = mediator;
      }

      /// <summary>Get a store's manual morning/night attendance hours and late-grace period.</summary>
      [HttpGet]
      public async Task<ActionResult<AttendanceSettingsDto>> Get(Guid storeId, CancellationToken cancellationToken)
      {
            var settings = await _mediator.Send(new GetAttendanceSettingsByStoreQuery(storeId), cancellationToken);
            return Ok(settings);
      }

      /// <summary>Create or update a store's attendance settings (morning/night hours, late grace period).</summary>
      [HttpPut]
      [Authorize(Roles = "StoreManager,AreaManager,Admin")]
      public async Task<ActionResult<Guid>> Upsert(
          Guid storeId, UpsertAttendanceSettingsCommand command, CancellationToken cancellationToken)
      {
            if (storeId != command.StoreId)
                  return BadRequest("Route storeId and body StoreId must match.");

            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
      }
}