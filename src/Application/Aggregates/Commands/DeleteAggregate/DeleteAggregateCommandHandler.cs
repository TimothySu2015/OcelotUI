using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Aggregates.Commands.DeleteAggregate;

public class DeleteAggregateCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<DeleteAggregateCommand, Result>
{
    public async Task<Result> Handle(
        DeleteAggregateCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var aggregates = config.Aggregates ?? [];

        if (request.Index < 0 || request.Index >= aggregates.Count)
            return Result.Failure($"Aggregate index {request.Index} is out of range.");

        aggregates.RemoveAt(request.Index);
        if (aggregates.Count == 0) config.Aggregates = null;
        await repository.SaveAsync(config, "DeleteAggregate", cancellationToken);

        return Result.Success();
    }
}
