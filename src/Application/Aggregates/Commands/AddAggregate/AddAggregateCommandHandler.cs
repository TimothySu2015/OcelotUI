using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Aggregates.Commands.AddAggregate;

public class AddAggregateCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<AddAggregateCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        AddAggregateCommand request, CancellationToken cancellationToken)
    {
        var agg = request.Aggregate.Aggregate;

        if (string.IsNullOrWhiteSpace(agg.UpstreamPathTemplate))
            return Result.Failure<int>("UpstreamPathTemplate is required.");

        if (agg.RouteKeys.Count == 0)
            return Result.Failure<int>("At least one RouteKey is required.");

        var config = await repository.LoadAsync(cancellationToken);
        config.Aggregates ??= [];
        config.Aggregates.Add(agg);
        await repository.SaveAsync(config, "AddAggregate", cancellationToken);

        return Result.Success(config.Aggregates.Count - 1);
    }
}
