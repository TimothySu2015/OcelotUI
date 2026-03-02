using System.Text.Json;
using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.DeprecatedProperties;

public class RemoveAllDeprecatedPropertiesCommandHandler(IOcelotConfigurationRepository repository)
    : IRequestHandler<RemoveAllDeprecatedPropertiesCommand, Result<int>>
{
    private static readonly HashSet<string> DeprecatedExtensionKeys = new(StringComparer.Ordinal)
    {
        "DurationOfBreak",
        "ExceptionsAllowedBeforeBreaking",
        "TimeoutValue",
        "DisableRateLimitHeaders",
        "HttpStatusCode",
        "QuotaExceededMessage",
        "RateLimitCounterPrefix",
        "PeriodTimespan",
        "FileCacheOptions",
    };

    public async Task<Result<int>> Handle(
        RemoveAllDeprecatedPropertiesCommand request, CancellationToken cancellationToken)
    {
        var config = await repository.LoadAsync(cancellationToken);
        var removed = 0;

        // Routes
        foreach (var route in config.Routes)
        {
            removed += CleanExtensionData(route.ExtensionData);
            removed += CleanSubObjects(route.QoSOptions, route.RateLimitOptions, route.AuthenticationOptions);
        }

        // GlobalConfiguration
        if (config.GlobalConfiguration is { } global)
        {
            removed += CleanExtensionData(global.ExtensionData);
            removed += CleanSubObjects(global.QoSOptions, global.RateLimitOptions, global.AuthenticationOptions);
        }

        // DynamicRoutes
        if (config.DynamicRoutes is { } dynamics)
        {
            foreach (var dr in dynamics)
            {
                removed += CleanExtensionData(dr.ExtensionData);
                removed += CleanSubObjects(dr.QoSOptions, dr.RateLimitOptions, dr.AuthenticationOptions);
            }
        }

        if (removed > 0)
        {
            await repository.SaveAsync(config, "RemoveDeprecatedProperties", cancellationToken);
        }

        return Result.Success(removed);
    }

    private static int CleanSubObjects(
        QoSOptions? qos, RateLimitOptions? rateLimit, AuthenticationOptions? auth)
    {
        var removed = 0;

        if (qos is not null)
            removed += CleanExtensionData(qos.ExtensionData);

        if (rateLimit is not null)
            removed += CleanExtensionData(rateLimit.ExtensionData);

        if (auth is not null)
        {
            removed += CleanExtensionData(auth.ExtensionData);

            if (auth.AuthenticationProviderKey is not null)
            {
                auth.AuthenticationProviderKey = null;
                removed++;
            }
        }

        return removed;
    }

    private static int CleanExtensionData(Dictionary<string, JsonElement>? extensionData)
    {
        if (extensionData is null or { Count: 0 })
            return 0;

        var removed = 0;
        foreach (var key in DeprecatedExtensionKeys)
        {
            if (extensionData.Remove(key))
                removed++;
        }

        return removed;
    }
}
