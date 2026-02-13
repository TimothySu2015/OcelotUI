using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Routes.Commands.UpdateRoute;

public class UpdateRouteCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<UpdateRouteCommand, Result>
{
    public async Task<Result> Handle(
        UpdateRouteCommand request, CancellationToken cancellationToken)
    {
        var route = request.Route.Route;

        if (string.IsNullOrWhiteSpace(route.UpstreamPathTemplate))
            return Result.Failure("UpstreamPathTemplate is required.");

        var config = await repository.LoadAsync(cancellationToken);

        if (request.Index < 0 || request.Index >= config.Routes.Count)
            return Result.Failure($"Route index {request.Index} is out of range.");

        config.Routes[request.Index] = route;
        await repository.SaveAsync(config, cancellationToken);

        return Result.Success();
    }
}
