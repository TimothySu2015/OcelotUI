using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Routes.Queries.GetAllRoutes;

public class GetAllRoutesQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetAllRoutesQuery, Result<List<RouteListItemDto>>>
{
    public async Task<Result<List<RouteListItemDto>>> Handle(
        GetAllRoutesQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);

        var items = config.Routes
            .Select((route, index) => new RouteListItemDto(
                Index: index,
                DisplayName: route.Metadata is not null && route.Metadata.TryGetValue("_displayName", out var dn) ? dn : null,
                UpstreamPathTemplate: route.UpstreamPathTemplate,
                UpstreamHttpMethod: route.UpstreamHttpMethod,
                DownstreamPathTemplate: route.DownstreamPathTemplate,
                DownstreamScheme: route.DownstreamScheme,
                DownstreamHosts: string.Join(", ",
                    route.DownstreamHostAndPorts.Select(h => $"{h.Host}:{h.Port}")),
                Key: route.Key))
            .ToList();

        return Result.Success(items);
    }
}
