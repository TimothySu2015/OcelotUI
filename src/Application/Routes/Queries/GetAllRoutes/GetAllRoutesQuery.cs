using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Routes.Queries.GetAllRoutes;

public record GetAllRoutesQuery : IRequest<Result<List<RouteListItemDto>>>;
