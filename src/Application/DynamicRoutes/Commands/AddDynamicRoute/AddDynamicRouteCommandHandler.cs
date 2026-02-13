using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.DynamicRoutes.Commands.AddDynamicRoute;

public class AddDynamicRouteCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<AddDynamicRouteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        AddDynamicRouteCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        config.DynamicRoutes ??= [];
        config.DynamicRoutes.Add(request.DynamicRoute.DynamicRoute);
        await repository.SaveAsync(config, cancellationToken);

        return Result.Success(config.DynamicRoutes.Count - 1);
    }
}
