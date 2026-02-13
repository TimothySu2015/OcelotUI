using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Routes.Queries.GetRouteByIndex;

public record GetRouteByIndexQuery(int Index) : IRequest<Result<RouteDetailDto>>;
