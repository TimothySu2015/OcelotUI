using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.DynamicRoutes.Commands.UpdateDynamicRoute;

public class UpdateDynamicRouteCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<UpdateDynamicRouteCommand, Result>
{
    public async Task<Result> Handle(
        UpdateDynamicRouteCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var dynamicRoutes = config.DynamicRoutes ?? [];

        if (request.Index < 0 || request.Index >= dynamicRoutes.Count)
            return Result.Failure($"DynamicRoute index {request.Index} is out of range.");

        dynamicRoutes[request.Index] = request.DynamicRoute.DynamicRoute;
        await repository.SaveAsync(config, "UpdateDynamicRoute", cancellationToken);

        return Result.Success();
    }
}
