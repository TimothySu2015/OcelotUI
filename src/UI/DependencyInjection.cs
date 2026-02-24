using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using OcelotUI.Application.Routes.Queries.GetAllRoutes;
using OcelotUI.Infrastructure;
using OcelotUI.UI.Services;

namespace OcelotUI.UI;

public static class DependencyInjection
{
    /// <summary>
    /// 集中註冊 OcelotUI 共用服務：MudBlazor、Localization、MediatR、JsonPreviewState、Infrastructure。
    /// ICultureSwitcher 由各 Host 自行註冊。
    /// </summary>
    public static IServiceCollection AddOcelotUI(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        });

        services.AddLocalization();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetAllRoutesQuery).Assembly));

        services.AddScoped<JsonPreviewState>();

        services.AddInfrastructure(configuration);

        return services;
    }
}
