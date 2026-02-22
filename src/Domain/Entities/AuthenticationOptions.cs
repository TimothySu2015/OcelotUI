using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotUI.Domain.Entities;

public class AuthenticationOptions
{
    public bool? AllowAnonymous { get; set; }
    public List<string>? AllowedScopes { get; set; }
    public string? AuthenticationProviderKey { get; set; }
    public List<string>? AuthenticationProviderKeys { get; set; }
    public List<string>? RouteKeys { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
