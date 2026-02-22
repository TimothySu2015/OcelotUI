using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class OcelotAggregate
{
    public string UpstreamPathTemplate { get; set; } = string.Empty;
    public List<string>? UpstreamHttpMethod { get; set; }
    public List<string> RouteKeys { get; set; } = [];
    public string? UpstreamHost { get; set; }
    public Dictionary<string, string>? UpstreamHeaderTemplates { get; set; }
    public bool? RouteIsCaseSensitive { get; set; }
    public int? Priority { get; set; }
    public string? Aggregator { get; set; }
    public List<AggregateRouteConfig>? RouteKeysConfig { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
