using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.Interfaces;

public interface IOcelotConfigurationRepository
{
    Task<OcelotConfiguration> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(OcelotConfiguration configuration, string? changeDescription = null, CancellationToken ct = default);
}
