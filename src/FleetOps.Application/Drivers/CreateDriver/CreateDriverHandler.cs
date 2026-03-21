using FleetOps.Domain.Drivers;
using FluentValidation;

namespace FleetOps.Application.Drivers.CreateDriver;

public sealed class CreateDriverHandler
{
    private readonly IDriverRepository _repository;
    private readonly IValidator<CreateDriverCommand> _validator;

    public CreateDriverHandler(
        IDriverRepository repository,
        IValidator<CreateDriverCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateDriverResult> HandleAsync(CreateDriverCommand command, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var driver = new Driver(command.Name);

        await _repository.AddAsync(driver, ct);
        await _repository.SaveChangesAsync(ct);

        return new CreateDriverResult(driver.Id);
    }

}