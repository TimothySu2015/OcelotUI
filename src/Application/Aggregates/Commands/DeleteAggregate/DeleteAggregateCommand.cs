using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Aggregates.Commands.DeleteAggregate;

public record DeleteAggregateCommand(int Index) : IRequest<Result>;
