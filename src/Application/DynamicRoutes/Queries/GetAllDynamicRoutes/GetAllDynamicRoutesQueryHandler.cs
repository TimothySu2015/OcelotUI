using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.DynamicRoutes.Queries.GetAllDynamicRoutes;

public class GetAllDynamicRoutesQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetAllDynamicRoutesQuery, Result<List<DynamicRouteListItemDto>>>
{
    public async Task<Result<List<DynamicRouteListItemDto>>> Handle(
        GetAllDynamicRoutesQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var dynamicRoutes = config.DynamicRoutes ?? [];

        var items = dynamicRoutes
            .Select((dr, index) => new DynamicRouteListItemDto(
                Index: index,
                ServiceName: dr.ServiceName,
                ServiceNamespace: dr.ServiceNamespace))
            .ToList();

        return Result.Success(items);
    }
}
