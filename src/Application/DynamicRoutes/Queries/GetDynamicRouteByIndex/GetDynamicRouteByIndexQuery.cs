using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.DynamicRoutes.Queries.GetDynamicRouteByIndex;

public record GetDynamicRouteByIndexQuery(int Index) : IRequest<Result<DynamicRouteDetailDto>>;
