using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Routes.Queries.GetRouteByIndex;

namespace OcelotUI.Application.Routes.Commands.UpdateRoute;

public record UpdateRouteCommand(int Index, RouteDetailDto Route) : IRequest<Result>;
