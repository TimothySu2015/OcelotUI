using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Aggregates.Queries.GetAllAggregates;

public record GetAllAggregatesQuery : IRequest<Result<List<AggregateListItemDto>>>;
