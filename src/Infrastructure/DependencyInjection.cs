using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OcelotUI.Application.Interfaces;
using OcelotUI.Infrastructure.Persistence;

namespace OcelotUI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OcelotConfigOptions>(
            configuration.GetSection(OcelotConfigOptions.SectionName));

        services.AddScoped<IOcelotConfigurationRepository, OcelotFileConfigurationRepository>();

        return services;
    }
}
