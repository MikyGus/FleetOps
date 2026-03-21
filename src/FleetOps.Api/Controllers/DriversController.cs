using FleetOps.Api.Contracts.Drivers;
using FleetOps.Application.Drivers;
using FleetOps.Application.Drivers.CreateDriver;
using FleetOps.Application.Drivers.GetDrivers;
using Microsoft.AspNetCore.Mvc;

namespace FleetOps.Api.Controllers;

[ApiController]
[Route("drivers")]
public sealed class DriversController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateDriverResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDriverRequest request,
        [FromServices] CreateDriverHandler handler,
        CancellationToken ct)
    {
        var command = new CreateDriverCommand(request.Name);

        CreateDriverResult result = await handler.HandleAsync(command, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            new CreateDriverResponse(result.Id));
    }

    [HttpGet]
    [ProducesResponseType<List<DriverDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<DriverDto>>> Get(
        [FromServices] GetDriversHandler handler,
        [FromQuery] string? name,
        [FromQuery] bool? isActive,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        CancellationToken ct = default)
    {
        var query = new GetDriversQuery(name, isActive, limit, offset);

        List<DriverDto> result = await handler.HandleAsync(query, ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<DriverDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverDto>> GetById(
        Guid id,
        [FromServices] GetDriverByIdHandler handler,
        CancellationToken ct)
    {
        DriverDto? driver = await handler.HandleAsync(id, ct);

        if (driver is null)
        {
            return NotFound();
        }

        return Ok(driver);
    }

}