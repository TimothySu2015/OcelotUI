using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.DynamicRoutes.Commands.DeleteDynamicRoute;

public record DeleteDynamicRouteCommand(int Index) : IRequest<Result>;
