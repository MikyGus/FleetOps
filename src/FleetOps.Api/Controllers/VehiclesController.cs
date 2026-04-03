using FleetOps.Api.Contracts.Vehicles;
using FleetOps.Application.Vehicles;
using FleetOps.Application.Vehicles.CreateVehicle;
using FleetOps.Application.Vehicles.GetVehicles;
using Microsoft.AspNetCore.Mvc;

namespace FleetOps.Api.Controllers;

[ApiController]
[Route("vehicles")]
public sealed class VehicleController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateVehicleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVehicleRequest request,
        [FromServices] CreateVehicleHandler handler,
        CancellationToken ct)
    {
        var command = new CreateVehicleCommand(request.RegistrationNumber);

        CreateVehicleResult result = await handler.HandleAsync(command, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            new CreateVehicleResponse(result.Id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<VehicleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] GetVehicleByIdHandler handler,
        CancellationToken ct)
    {
        VehicleDto? vehicle = await handler.HandleAsync(id, ct);

        if (vehicle is null)
        {
            return NotFound();
        }

        return Ok(vehicle);
    }

    [HttpGet]
    [ProducesResponseType<List<VehicleDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<VehicleDto>>> GetVehicles(
        [FromServices] GetVehiclesHandler handler,
        [FromQuery] string? registrationnumber,
        [FromQuery] bool? isActive,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        CancellationToken ct = default
    )
    {
        var query = new GetVehiclesQuery(registrationnumber, isActive, limit, offset);

        List<VehicleDto> result = await handler.HandleAsync(query, ct);

        return Ok(result);
    }
}