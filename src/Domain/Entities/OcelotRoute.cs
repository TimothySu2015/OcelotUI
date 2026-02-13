using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class OcelotRoute
{
    // Core Upstream
    public string UpstreamPathTemplate { get; set; } = string.Empty;
    public List<string> UpstreamHttpMethod { get; set; } = [];
    public string? UpstreamHost { get; set; }

    // Core Downstream
    public string DownstreamPathTemplate { get; set; } = string.Empty;
    public string DownstreamScheme { get; set; } = "https";
    public List<DownstreamHostAndPort> DownstreamHostAndPorts { get; set; } = [];
    public string? DownstreamHttpMethod { get; set; }
    public string? DownstreamHttpVersion { get; set; }
    public string? DownstreamHttpVersionPolicy { get; set; }

    // Options
    public string? Key { get; set; }
    public int? Priority { get; set; }
    public bool? RouteIsCaseSensitive { get; set; }
    public int? Timeout { get; set; }
    public bool? DangerousAcceptAnyServerCertificateValidator { get; set; }
    public string? RequestIdKey { get; set; }
    public string? ServiceName { get; set; }
    public string? ServiceNamespace { get; set; }

    // Complex sub-objects (strongly typed)
    public AuthenticationOptions? AuthenticationOptions { get; set; }
    public RateLimitOptions? RateLimitOptions { get; set; }
    public LoadBalancerOptions? LoadBalancerOptions { get; set; }
    public QoSOptions? QoSOptions { get; set; }
    public CacheOptions? CacheOptions { get; set; }
    public HttpHandlerOptions? HttpHandlerOptions { get; set; }
    public SecurityOptions? SecurityOptions { get; set; }

    // Dictionary fields
    public Dictionary<string, string>? Metadata { get; set; }
    public Dictionary<string, string>? AddHeadersToRequest { get; set; }
    public Dictionary<string, string>? AddClaimsToRequest { get; set; }
    public Dictionary<string, string>? AddQueriesToRequest { get; set; }
    public Dictionary<string, string>? ChangeDownstreamPathTemplate { get; set; }
    public Dictionary<string, string>? UpstreamHeaderTemplates { get; set; }
    public Dictionary<string, string>? UpstreamHeaderTransform { get; set; }
    public Dictionary<string, string>? DownstreamHeaderTransform { get; set; }
    public Dictionary<string, string>? RouteClaimsRequirement { get; set; }

    // Array fields
    public List<string>? DelegatingHandlers { get; set; }

    // Catch-all for unknown properties
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
