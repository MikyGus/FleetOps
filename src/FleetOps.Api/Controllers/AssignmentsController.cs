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
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request, CancellationToken ct)
    {
        var command = new CreateAssignmentCommand(
            request.DriverId,
            request.VehicleId,
            request.StartUtc,
            request.EndUtc
        );

        CreateAssignmentResult result = await _handler.HandleAsync(command, ct);

        return Created($"/assignments/{result.Id}", new CreateAssignmentResponse(result.Id));
    }
}