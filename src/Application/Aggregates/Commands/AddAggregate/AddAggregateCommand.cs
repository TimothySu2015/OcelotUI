using MediatR;
using OcelotUI.Application.Aggregates.Queries.GetAggregateByIndex;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Aggregates.Commands.AddAggregate;

public record AddAggregateCommand(AggregateDetailDto Aggregate) : IRequest<Result<int>>;
