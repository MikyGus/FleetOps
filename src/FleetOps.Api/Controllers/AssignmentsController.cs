using FleetOps.Api.Contracts.Assignments;
using FleetOps.Application.Assignments.CreateAssignment;
using Microsoft.AspNetCore.Mvc;

namespace FleetOps.Api.Controllers;

[ApiController]
[Route("assignments")]
public sealed class AssignmentsController : ControllerBase
{
    private readonly CreateAssignmentHandler _handler;

    public AssignmentsController(CreateAssignmentHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<CreateAssignmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request, CancellationToken ct)
    {
        var command = new CreateAssignmentCommand(
            request.DriverId,
            request.VehicleId,
            request.StartUtc,
            request.EndUtc
        );

        CreateAssignmentResult result = await _handler.HandleAsync(command, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            new CreateAssignmentResponse(result.Id)
        );
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(); // Placeholder
    }
}