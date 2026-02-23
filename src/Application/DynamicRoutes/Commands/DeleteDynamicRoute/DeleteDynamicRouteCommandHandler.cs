using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.DynamicRoutes.Commands.DeleteDynamicRoute;

public class DeleteDynamicRouteCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<DeleteDynamicRouteCommand, Result>
{
    public async Task<Result> Handle(
        DeleteDynamicRouteCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var dynamicRoutes = config.DynamicRoutes ?? [];

        if (request.Index < 0 || request.Index >= dynamicRoutes.Count)
            return Result.Failure($"DynamicRoute index {request.Index} is out of range.");

        dynamicRoutes.RemoveAt(request.Index);
        if (dynamicRoutes.Count == 0) config.DynamicRoutes = null;
        await repository.SaveAsync(config, "DeleteDynamicRoute", cancellationToken);

        return Result.Success();
    }
}
