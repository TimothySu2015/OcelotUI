namespace OcelotUI.UI.Services;

/// <summary>
/// Ocelot 24.1 已棄用屬性的替代對照資訊。
/// </summary>
public record DeprecatedPropertyInfo(string Replacement, string DescriptionKey);

/// <summary>
/// Ocelot 24.1 棄用屬性對照表。這些屬性在 v25.0 將被移除。
/// </summary>
public static class DeprecatedPropertyMap
{
    public static IReadOnlyDictionary<string, DeprecatedPropertyInfo> Entries { get; } =
        new Dictionary<string, DeprecatedPropertyInfo>(StringComparer.Ordinal)
        {
            // QoSOptions
            ["DurationOfBreak"] = new("BreakDuration", "Deprecated_QoS_DurationOfBreak"),
            ["ExceptionsAllowedBeforeBreaking"] = new("MinimumThroughput", "Deprecated_QoS_ExceptionsAllowedBeforeBreaking"),
            ["TimeoutValue"] = new("Timeout", "Deprecated_QoS_TimeoutValue"),

            // RateLimitOptions (global)
            ["DisableRateLimitHeaders"] = new("EnableHeaders", "Deprecated_RateLimit_DisableRateLimitHeaders"),
            ["HttpStatusCode"] = new("StatusCode", "Deprecated_RateLimit_HttpStatusCode"),
            ["QuotaExceededMessage"] = new("QuotaMessage", "Deprecated_RateLimit_QuotaExceededMessage"),
            ["RateLimitCounterPrefix"] = new("KeyPrefix", "Deprecated_RateLimit_RateLimitCounterPrefix"),
            ["PeriodTimespan"] = new("Wait", "Deprecated_RateLimit_PeriodTimespan"),

            // AuthenticationOptions
            ["AuthenticationProviderKey"] = new("AuthenticationProviderKeys", "Deprecated_Auth_AuthenticationProviderKey"),

            // Route
            ["FileCacheOptions"] = new("CacheOptions", "Deprecated_Route_FileCacheOptions"),
        };
}
