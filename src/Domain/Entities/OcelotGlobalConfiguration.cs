using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class OcelotGlobalConfiguration
{
    // Basic settings
    public string? BaseUrl { get; set; }
    public string? DownstreamScheme { get; set; }
    public string? DownstreamHttpVersion { get; set; }
    public string? DownstreamHttpVersionPolicy { get; set; }
    public string? RequestIdKey { get; set; }
    public int? Timeout { get; set; }

    // Sub-objects (shared types with Route)
    public AuthenticationOptions? AuthenticationOptions { get; set; }
    public RateLimitOptions? RateLimitOptions { get; set; }
    public LoadBalancerOptions? LoadBalancerOptions { get; set; }
    public QoSOptions? QoSOptions { get; set; }
    public CacheOptions? CacheOptions { get; set; }
    public HttpHandlerOptions? HttpHandlerOptions { get; set; }
    public SecurityOptions? SecurityOptions { get; set; }

    // Global-specific
    public ServiceDiscoveryProvider? ServiceDiscoveryProvider { get; set; }
    public MetadataOptions? MetadataOptions { get; set; }

    // Dictionary fields
    public Dictionary<string, string>? Metadata { get; set; }
    public Dictionary<string, string>? UpstreamHeaderTransform { get; set; }
    public Dictionary<string, string>? DownstreamHeaderTransform { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
