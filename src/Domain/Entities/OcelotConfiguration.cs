using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class OcelotConfiguration
{
    public List<OcelotRoute> Routes { get; set; } = [];
    public List<OcelotDynamicRoute>? DynamicRoutes { get; set; }
    public List<OcelotAggregate>? Aggregates { get; set; }
    public OcelotGlobalConfiguration? GlobalConfiguration { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
