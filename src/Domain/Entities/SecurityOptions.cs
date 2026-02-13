using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class SecurityOptions
{
    public List<string>? IPAllowedList { get; set; }
    public List<string>? IPBlockedList { get; set; }
    public bool? ExcludeAllowedFromBlocked { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
