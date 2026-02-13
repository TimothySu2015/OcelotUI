using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.GlobalConfig.Queries.GetGlobalConfig;

public class GetGlobalConfigQueryHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<GetGlobalConfigQuery, Result<GlobalConfigDto>>
{
    public async Task<Result<GlobalConfigDto>> Handle(
        GetGlobalConfigQuery request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var globalConfig = config.GlobalConfiguration ?? new OcelotGlobalConfiguration();

        return Result.Success(new GlobalConfigDto { Config = globalConfig });
    }
}
