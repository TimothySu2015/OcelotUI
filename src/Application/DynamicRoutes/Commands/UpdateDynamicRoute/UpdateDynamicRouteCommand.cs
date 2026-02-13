using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.DynamicRoutes.Queries.GetDynamicRouteByIndex;

namespace OcelotUI.Application.DynamicRoutes.Commands.UpdateDynamicRoute;

public record UpdateDynamicRouteCommand(int Index, DynamicRouteDetailDto DynamicRoute) : IRequest<Result>;
