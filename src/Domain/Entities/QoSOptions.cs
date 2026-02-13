using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class QoSOptions
{
    public int? BreakDuration { get; set; }
    public int? MinimumThroughput { get; set; }
    public double? FailureRatio { get; set; }
    public int? SamplingDuration { get; set; }
    public int? Timeout { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
