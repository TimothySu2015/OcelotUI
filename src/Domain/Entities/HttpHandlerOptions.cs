using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class HttpHandlerOptions
{
    public bool? AllowAutoRedirect { get; set; }
    public int? MaxConnectionsPerServer { get; set; }
    public int? PooledConnectionLifetimeSeconds { get; set; }
    public bool? UseCookieContainer { get; set; }
    public bool? UseProxy { get; set; }
    public bool? UseTracing { get; set; }
    public List<string>? RouteKeys { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
