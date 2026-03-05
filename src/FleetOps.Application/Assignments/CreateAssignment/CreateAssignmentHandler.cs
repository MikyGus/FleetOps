using FleetOps.Application.Assignments;
using FleetOps.Domain.Assignments;
using FluentValidation;

namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentHandler
{
    private readonly IAssignmentRepository _repo;
    private readonly IValidator<CreateAssignmentCommand> _validator;

    public CreateAssignmentHandler(
        IAssignmentRepository repo,
        IValidator<CreateAssignmentCommand> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<CreateAssignmentResult> HandleAsync(
        CreateAssignmentCommand command, 
        CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var assignment = new Assignment(
            command.DriverId,
            command.VehicleId,
            command.StartUtc,
            command.EndUtc);
        
        await _repo.AddAsync(assignment, ct);
        await _repo.SaveChangesAsync(ct);

        return new CreateAssignmentResult(assignment.Id);
    }
}