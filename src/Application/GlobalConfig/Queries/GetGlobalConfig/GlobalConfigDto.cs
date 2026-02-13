using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.GlobalConfig.Queries.GetGlobalConfig;

public class GlobalConfigDto
{
    public OcelotGlobalConfiguration Config { get; set; } = new();
}
