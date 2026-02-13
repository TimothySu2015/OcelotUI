using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.DynamicRoutes.Queries.GetDynamicRouteByIndex;

namespace OcelotUI.Application.DynamicRoutes.Commands.AddDynamicRoute;

public record AddDynamicRouteCommand(DynamicRouteDetailDto DynamicRoute) : IRequest<Result<int>>;
