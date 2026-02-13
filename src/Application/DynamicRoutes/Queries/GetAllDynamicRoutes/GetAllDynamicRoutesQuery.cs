using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.DynamicRoutes.Queries.GetAllDynamicRoutes;

public record GetAllDynamicRoutesQuery : IRequest<Result<List<DynamicRouteListItemDto>>>;
