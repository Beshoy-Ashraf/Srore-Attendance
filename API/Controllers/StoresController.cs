using Application.Stores.Commands.CreateStore;
using Application.Stores.Commands.DeleteStore;
using Application.Stores.Commands.UpdateStore;
using Application.Stores.Dtos;
using Application.Stores.Queries.GetStoreById;
using Application.Stores.Queries.GetStores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoresController(ISender mediator) : ControllerBase
{
      private readonly ISender _mediator = mediator;

      [HttpGet("{id:guid}")]
      public async Task<ActionResult<StoreDto>> GetById(Guid id, CancellationToken cancellationToken)
      {
            var store = await _mediator.Send(new GetStoreByIdQuery(id), cancellationToken);
            return Ok(store);
      }

      [HttpGet]
      public async Task<ActionResult<IEnumerable<StoreDto>>> GetAll(
          [FromQuery] Guid? areaManagerId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          CancellationToken cancellationToken = default)
      {
            var stores = await _mediator.Send(new GetStoresQuery(areaManagerId, page, pageSize), cancellationToken);
            return Ok(stores);
      }

      [HttpPost]
      [Authorize(Roles = "Admin,AreaManager")]
      public async Task<ActionResult<Guid>> Create(CreateStoreCommand command, CancellationToken cancellationToken)
      {
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
      }

      [HttpPut("{id:guid}")]
      [Authorize(Roles = "Admin,AreaManager")]
      public async Task<IActionResult> Update(Guid id, UpdateStoreCommand command, CancellationToken cancellationToken)
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
            await _mediator.Send(new DeleteStoreCommand(id), cancellationToken);
            return NoContent();
      }
}