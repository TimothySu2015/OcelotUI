using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class RateLimitOptions
{
    public bool? EnableRateLimiting { get; set; }
    public string? ClientIdHeader { get; set; }
    public List<string>? ClientWhitelist { get; set; }
    public bool? EnableHeaders { get; set; }
    public int? Limit { get; set; }
    public string? Period { get; set; }
    public string? Wait { get; set; }
    public int? StatusCode { get; set; }
    public string? QuotaMessage { get; set; }
    public string? KeyPrefix { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
