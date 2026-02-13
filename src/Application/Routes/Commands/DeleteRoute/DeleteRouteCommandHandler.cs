using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Routes.Commands.DeleteRoute;

public class DeleteRouteCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<DeleteRouteCommand, Result>
{
    public async Task<Result> Handle(
        DeleteRouteCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);

        if (request.Index < 0 || request.Index >= config.Routes.Count)
            return Result.Failure($"Route index {request.Index} is out of range.");

        config.Routes.RemoveAt(request.Index);
        await repository.SaveAsync(config, cancellationToken);

        return Result.Success();
    }
}
