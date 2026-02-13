using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class AggregateRouteConfig
{
    public string RouteKey { get; set; } = string.Empty;
    public string? JsonPath { get; set; }
    public string? Parameter { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
