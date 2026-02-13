using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Aggregates.Commands.UpdateAggregate;

public class UpdateAggregateCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<UpdateAggregateCommand, Result>
{
    public async Task<Result> Handle(
        UpdateAggregateCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var aggregates = config.Aggregates ?? [];

        if (request.Index < 0 || request.Index >= aggregates.Count)
            return Result.Failure($"Aggregate index {request.Index} is out of range.");

        aggregates[request.Index] = request.Aggregate.Aggregate;
        await repository.SaveAsync(config, cancellationToken);

        return Result.Success();
    }
}
