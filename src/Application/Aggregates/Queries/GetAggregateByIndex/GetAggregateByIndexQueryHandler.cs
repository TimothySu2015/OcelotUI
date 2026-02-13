using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Aggregates.Queries.GetAggregateByIndex;

public class GetAggregateByIndexQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetAggregateByIndexQuery, Result<AggregateDetailDto>>
{
    public async Task<Result<AggregateDetailDto>> Handle(
        GetAggregateByIndexQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var aggregates = config.Aggregates ?? [];

        if (request.Index < 0 || request.Index >= aggregates.Count)
            return Result.Failure<AggregateDetailDto>($"Aggregate index {request.Index} is out of range.");

        return Result.Success(AggregateDetailDto.FromEntity(request.Index, aggregates[request.Index]));
    }
}
