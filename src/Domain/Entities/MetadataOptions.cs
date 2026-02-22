using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class MetadataOptions
{
    public string? CurrentCulture { get; set; }
    public string? NumberStyle { get; set; }
    public List<string>? Separators { get; set; }
    public string? StringSplitOption { get; set; }
    public List<string>? TrimChars { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
