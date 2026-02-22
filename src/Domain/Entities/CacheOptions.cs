using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class CacheOptions
{
    public int? TtlSeconds { get; set; }
    public string? Region { get; set; }
    public string? Header { get; set; }
    public bool? EnableContentHashing { get; set; }
    public List<string>? RouteKeys { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
