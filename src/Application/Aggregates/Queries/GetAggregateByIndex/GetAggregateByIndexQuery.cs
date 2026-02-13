using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Aggregates.Queries.GetAggregateByIndex;

public record GetAggregateByIndexQuery(int Index) : IRequest<Result<AggregateDetailDto>>;
