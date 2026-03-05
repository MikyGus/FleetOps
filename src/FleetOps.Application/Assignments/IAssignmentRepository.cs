using FleetOps.Domain.Assignments;
namespace FleetOps.Application.Assignments;

public interface IAssignmentRepository
{
    Task AddAsync(Assignment assignment, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct); 
}