using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Aggregates.Queries.GetAllAggregates;

public class GetAllAggregatesQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetAllAggregatesQuery, Result<List<AggregateListItemDto>>>
{
    public async Task<Result<List<AggregateListItemDto>>> Handle(
        GetAllAggregatesQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var aggregates = config.Aggregates ?? [];

        var items = aggregates
            .Select((agg, index) => new AggregateListItemDto(
                Index: index,
                UpstreamPathTemplate: agg.UpstreamPathTemplate,
                RouteKeys: agg.RouteKeys,
                Aggregator: agg.Aggregator))
            .ToList();

        return Result.Success(items);
    }
}
