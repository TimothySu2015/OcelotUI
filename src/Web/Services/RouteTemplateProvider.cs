using OcelotUI.Domain.Entities;

namespace OcelotUI.Web.Services;

public record RouteTemplate(string NameKey, string DescriptionKey, OcelotRoute Route);

public static class RouteTemplateProvider
{
    public static IReadOnlyList<RouteTemplate> Templates { get; } =
    [
        new("Template_BasicRestApi", "Template_BasicRestApi_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/api/users/{everything}",
            UpstreamHttpMethod = ["GET", "POST", "PUT", "DELETE"],
            DownstreamPathTemplate = "/api/users/{everything}",
            DownstreamScheme = "https",
            DownstreamHostAndPorts = [new() { Host = "user-service", Port = 443 }]
        }),

        new("Template_SingleGet", "Template_SingleGet_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/api/health",
            UpstreamHttpMethod = ["GET"],
            DownstreamPathTemplate = "/health",
            DownstreamScheme = "https",
            DownstreamHostAndPorts = [new() { Host = "backend-service", Port = 443 }]
        }),

        new("Template_WebSocket", "Template_WebSocket_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/ws/{everything}",
            UpstreamHttpMethod = ["GET"],
            DownstreamPathTemplate = "/ws/{everything}",
            DownstreamScheme = "wss",
            DownstreamHostAndPorts = [new() { Host = "ws-service", Port = 443 }]
        }),

        new("Template_CatchAll", "Template_CatchAll_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/service-a/{everything}",
            UpstreamHttpMethod = ["GET", "POST", "PUT", "DELETE", "PATCH"],
            DownstreamPathTemplate = "/{everything}",
            DownstreamScheme = "https",
            DownstreamHostAndPorts = [new() { Host = "service-a.internal", Port = 443 }]
        }),

        new("Template_RateLimited", "Template_RateLimited_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/api/public/{everything}",
            UpstreamHttpMethod = ["GET"],
            DownstreamPathTemplate = "/api/{everything}",
            DownstreamScheme = "https",
            DownstreamHostAndPorts = [new() { Host = "public-api", Port = 443 }],
            RateLimitOptions = new RateLimitOptions
            {
                EnableRateLimiting = true,
                Period = "1m",
                Limit = 100
            }
        }),

        new("Template_Authenticated", "Template_Authenticated_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/api/secure/{everything}",
            UpstreamHttpMethod = ["GET", "POST", "PUT", "DELETE"],
            DownstreamPathTemplate = "/api/{everything}",
            DownstreamScheme = "https",
            DownstreamHostAndPorts = [new() { Host = "secure-service", Port = 443 }],
            AuthenticationOptions = new AuthenticationOptions
            {
                AuthenticationProviderKeys = ["Bearer"],
                AllowedScopes = []
            }
        }),

        new("Template_LoadBalanced", "Template_LoadBalanced_Desc", new OcelotRoute
        {
            UpstreamPathTemplate = "/api/products/{everything}",
            UpstreamHttpMethod = ["GET", "POST", "PUT", "DELETE"],
            DownstreamPathTemplate = "/api/products/{everything}",
            DownstreamScheme = "https",
            DownstreamHostAndPorts =
            [
                new() { Host = "product-service-1", Port = 443 },
                new() { Host = "product-service-2", Port = 443 },
                new() { Host = "product-service-3", Port = 443 }
            ],
            LoadBalancerOptions = new LoadBalancerOptions
            {
                Type = "RoundRobin"
            }
        })
    ];
}
