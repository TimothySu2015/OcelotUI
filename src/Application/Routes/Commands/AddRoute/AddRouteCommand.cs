using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Routes.Queries.GetRouteByIndex;

namespace OcelotUI.Application.Routes.Commands.AddRoute;

public record AddRouteCommand(RouteDetailDto Route) : IRequest<Result<int>>;
