using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Routes.Commands.DeleteRoute;

public record DeleteRouteCommand(int Index) : IRequest<Result>;
