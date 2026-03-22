using FleetOps.Api.Contracts.Vehicles;
using FleetOps.Application.Vehicles.CreateVehicle;
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
    public IActionResult GetById(Guid id)
    {
        return Ok(); // Placeholder
    }

}