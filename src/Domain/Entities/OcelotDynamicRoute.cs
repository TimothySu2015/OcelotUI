using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class OcelotDynamicRoute
{
    public string? ServiceName { get; set; }
    public string? ServiceNamespace { get; set; }
    public string? DownstreamHttpVersion { get; set; }
    public string? DownstreamHttpVersionPolicy { get; set; }
    public int? Timeout { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public AuthenticationOptions? AuthenticationOptions { get; set; }
    public RateLimitOptions? RateLimitOptions { get; set; }
    public LoadBalancerOptions? LoadBalancerOptions { get; set; }
    public QoSOptions? QoSOptions { get; set; }
    public CacheOptions? CacheOptions { get; set; }
    public HttpHandlerOptions? HttpHandlerOptions { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
