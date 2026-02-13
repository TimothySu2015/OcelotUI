using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.DynamicRoutes.Queries.GetDynamicRouteByIndex;

public class GetDynamicRouteByIndexQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetDynamicRouteByIndexQuery, Result<DynamicRouteDetailDto>>
{
    public async Task<Result<DynamicRouteDetailDto>> Handle(
        GetDynamicRouteByIndexQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var dynamicRoutes = config.DynamicRoutes ?? [];

        if (request.Index < 0 || request.Index >= dynamicRoutes.Count)
            return Result.Failure<DynamicRouteDetailDto>($"DynamicRoute index {request.Index} is out of range.");

        return Result.Success(DynamicRouteDetailDto.FromEntity(request.Index, dynamicRoutes[request.Index]));
    }
}
