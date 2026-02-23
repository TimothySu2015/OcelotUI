using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.GlobalConfig.Commands.UpdateGlobalConfig;

public class UpdateGlobalConfigCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<UpdateGlobalConfigCommand, Result>
{
    public async Task<Result> Handle(
        UpdateGlobalConfigCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        config.GlobalConfiguration = request.Config.Config;
        await repository.SaveAsync(config, "UpdateGlobalConfig", cancellationToken);

        return Result.Success();
    }
}
