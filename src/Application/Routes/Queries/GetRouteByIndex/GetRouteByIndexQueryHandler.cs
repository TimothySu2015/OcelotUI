using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Routes.Queries.GetRouteByIndex;

public class GetRouteByIndexQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetRouteByIndexQuery, Result<RouteDetailDto>>
{
    public async Task<Result<RouteDetailDto>> Handle(
        GetRouteByIndexQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);

        if (request.Index < 0 || request.Index >= config.Routes.Count)
            return Result.Failure<RouteDetailDto>($"Route index {request.Index} is out of range.");

        return Result.Success(RouteDetailDto.FromEntity(request.Index, config.Routes[request.Index]));
    }
}
