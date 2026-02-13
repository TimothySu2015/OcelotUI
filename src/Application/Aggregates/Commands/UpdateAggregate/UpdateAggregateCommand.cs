using MediatR;
using OcelotUI.Application.Aggregates.Queries.GetAggregateByIndex;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Aggregates.Commands.UpdateAggregate;

public record UpdateAggregateCommand(int Index, AggregateDetailDto Aggregate) : IRequest<Result>;
