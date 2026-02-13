using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class ServiceDiscoveryProvider
{
    public string? Type { get; set; }
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? Scheme { get; set; }
    public string? Token { get; set; }
    public string? ConfigurationKey { get; set; }
    public int? PollingInterval { get; set; }
    public string? Namespace { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
