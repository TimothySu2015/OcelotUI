using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Routes.Commands.AddRoute;

public class AddRouteCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<AddRouteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        AddRouteCommand request, CancellationToken cancellationToken)
    {
        var route = request.Route.Route;

        if (string.IsNullOrWhiteSpace(route.UpstreamPathTemplate))
            return Result.Failure<int>("UpstreamPathTemplate is required.");

        if (route.DownstreamHostAndPorts.Count == 0)
            return Result.Failure<int>("At least one DownstreamHostAndPort is required.");

        var config = await repository.LoadAsync(cancellationToken);
        config.Routes.Add(route);
        await repository.SaveAsync(config, cancellationToken);

        return Result.Success(config.Routes.Count - 1);
    }
}
