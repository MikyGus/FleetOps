using FleetOps.Api.Contracts.Assignments;
using FleetOps.Application.Assignments.CreateAssignment;
using FleetOps.Application.Assignments.GetAssignments;
using Microsoft.AspNetCore.Mvc;

namespace FleetOps.Api.Controllers;

[ApiController]
[Route("assignments")]
public sealed class AssignmentsController : ControllerBase
{
    private readonly CreateAssignmentHandler _createHandler;
    private readonly GetAssignmentsHandler _getHandler;
    private readonly GetAssignmentByIdHandler _getByIdHandler;


    public AssignmentsController(
        CreateAssignmentHandler createHandler,
        GetAssignmentsHandler getHandler,
        GetAssignmentByIdHandler getByIdHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _getByIdHandler = getByIdHandler;
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

        CreateAssignmentResult result = await _createHandler.HandleAsync(command, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            new CreateAssignmentResponse(result.Id)
        );
    }

    [HttpGet]
    [ProducesResponseType<List<AssignmentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AssignmentDto>>> Get(
        [FromQuery] Guid? driverId,
        [FromQuery] Guid? vehicleId,
        [FromQuery] DateTimeOffset? fromUtc,
        [FromQuery] DateTimeOffset? toUtc,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        CancellationToken ct = default)
    {
        var query = new GetAssignmentsQuery(driverId, vehicleId, fromUtc, toUtc, limit, offset);

        List<AssignmentDto> result = await _getHandler.HandleAsync(query, ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AssignmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignmentDto>> GetById(Guid id, CancellationToken ct)
    {
        AssignmentDto? assignment = await _getByIdHandler.HandleAsync(id, ct);
         
        if (assignment is null)
        {
            return NotFound();
        }

        return Ok(assignment);
    }
}