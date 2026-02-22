# Ocelot 24.1 — 官方屬性說明 (Property Descriptions)

> 來源：https://ocelot.readthedocs.io/en/latest/
> 擷取日期：2026-02-14
> 用途：UI 編輯器的 tooltip / help text 參考

---

## CacheOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `TtlSeconds` | Time-To-Live (TTL) in seconds for the cached downstream response, with absolute expiration starting when added to cache. If undefined/0, caching is disabled. | `int?`, default `0` |
| `Region` | Specifies which cache region to clear via the Administration API using `DELETE {adminPath}/outputcache/{region}`. | `string`, default `""` |
| `Header` | Specifies the header name used for native Ocelot caching control. Header values are included in the cache key; varying values create different cache keys. | `string`, default `"OC-Cache-Control"` |
| `EnableContentHashing` | Toggles inclusion of request body hashing in the cache key. Disabled by default due to performance impact; recommended for POST/PUT routes where request body affects the response. | `bool?`, default `false` |

> `FileCacheOptions` is deprecated (v24.1+); use `CacheOptions` instead.

---

## QoSOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `Timeout` | Default timeout in milliseconds for downstream requests. | `int`, 10–86,400,000 ms, default `30000` |
| `BreakDuration` | Duration of break the circuit will stay open before resetting. | `int`, 500–86,400,000 ms, default `5000` |
| `MinimumThroughput` | Number of actions required within sampling window before circuit breaker engages. | `int`, ≥ 2, default `100` |
| `FailureRatio` | Failure-to-success ratio at which the circuit will break. | `float`, > 0.0 and ≤ 1.0, default `0.1` (10%) |
| `SamplingDuration` | Duration of the sampling over which failure ratios are assessed. | `int`, 500–86,400,000 ms, default `30000` |

**Deprecated (removed in v25.0):**

| 已棄用 | 替代 | 說明 |
|--------|------|------|
| `DurationOfBreak` | `BreakDuration` | — |
| `ExceptionsAllowedBeforeBreaking` | `MinimumThroughput` | — |
| `TimeoutValue` | `Timeout` | — |

**Timeout 優先順序:** QoS Timeout (ms) > Route Timeout (s) > Global Timeout (s) > 90s default

---

## LoadBalancerOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `Type` | An in-built load balancer type selected from the list of available Balancers, or a user-defined type. **(mandatory)** | `string` |
| `Key` | The name of the cookie you wish to use for sticky sessions. This option is applicable only to the `CookieStickySessions` type. | `string` |
| `Expiry` | Expiration period specifies how long, in milliseconds, the session should remain sticky. This value refreshes with each request to mimic typical session behavior. Only applies to `CookieStickySessions`. | `int`, default `1200000` (20 min) |

### Type 選項說明

| 值 | 官方說明 |
|----|---------|
| `LeastConnection` | Tracks which services are dealing with requests and sends new requests to the service with the fewest existing requests. Algorithm state isn't distributed across Ocelot clusters. |
| `RoundRobin` | Loops through available services and sends requests. Algorithm state remains local to each Ocelot instance. |
| `CookieStickySessions` | Uses a cookie to stick all requests to a specific server for managing session state across non-shared downstream servers. |
| `NoLoadBalancer` | Takes the first available service from configuration or Service Discovery provider, effectively disabling load balancing. |

---

## RateLimitOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `EnableRateLimiting` | Controls whether rate limiting applies to the route. | `bool?`, default `true` |
| `Limit` | Maximum requests permitted within the specified Period. | `long?`, **required** |
| `Period` | Time window for counting requests. Accepts floating-point numbers followed by units: `ms`, `s`, `m`, `h`, `d`. Examples: "333.5ms", "10s", "1.5m", "20s". | `string`, **required** |
| `Wait` | Duration blocking requests after quota exhaustion. Same time format as Period. Example: "1.5m". | `string`, optional |
| `ClientIdHeader` | Identifies which HTTP header contains the client identifier. | `string`, default `"Oc-Client"` |
| `ClientWhitelist` | List of client identifiers exempted from rate limiting. | `string[]`, default `[]` |
| `EnableHeaders` | Determines if `X-RateLimit-*` and `Retry-After` headers are included in responses. | `bool?`, default `true` |
| `StatusCode` | HTTP status code for rate limit rejections. | `int?`, default `429` |
| `QuotaMessage` | Custom message when quota is exceeded. Supports `{0}` (request count) and `{1}` (period) placeholders. | `string`, default informative message |
| `KeyPrefix` | Cache key prefix for rate limit counters. | `string`, default `"Ocelot.RateLimiting"` |

**Period/Wait 時間單位:**
- `ms` — milliseconds
- `s` — seconds
- `m` — minutes
- `h` — hours
- `d` — days

**Deprecated (removed in v25.0):**

| 已棄用 | 替代 | 說明 |
|--------|------|------|
| `DisableRateLimitHeaders` | `EnableHeaders` | Boolean inverted; if both set, deprecated takes precedence |
| `HttpStatusCode` | `StatusCode` | — |
| `QuotaExceededMessage` | `QuotaMessage` | — |
| `RateLimitCounterPrefix` | `KeyPrefix` | — |
| `PeriodTimespan` | `Wait` | Was in seconds (double) |

**Global-level additional property:**

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `RouteKeys` | Specifies which routes receive global settings; empty array applies to all routes. | `string[]`, default `[]` |

---

## AuthenticationOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `AuthenticationProviderKeys` | **(Recommended)** Maps multiple authentication provider schemes to a route. Enforces "First One Wins" strategy based on array order. Mandatory when `AuthenticationProviderKey` is absent. | `string[]` |
| `AuthenticationProviderKey` | **(Deprecated v24.1, removed v25.0)** Maps a single authentication provider (by scheme/key) to a route. Use `AuthenticationProviderKeys` instead. | `string` |
| `AllowedScopes` | When specified, enforces authorization based on the `scope` claim after successful authentication. Requires at least one matching scope from the token. Supports RFC 8693 (OAuth 2.0 Token Exchange) for space-separated scope claims per JWT standards. | `string[]`, default `[]` |
| `AllowAnonymous` | Excludes a route from global authentication requirements when set to `true`. If global config enforces authentication, route-level `false` includes that route in authentication. | `bool?`, default `false` |

**Behavior:**
- Multiple Schemes attempted in array order (First One Wins)
- Failed authentication returns **401 Unauthorized**
- Scope validation is post-authentication

---

## HttpHandlerOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `AllowAutoRedirect` | Follow HTTP 3xx redirects automatically. | `bool`, default `false` |
| `UseCookieContainer` | Enable cookie persistence across requests to the downstream service. | `bool`, default `false` |
| `UseProxy` | Use system proxy settings for downstream requests. | `bool`, default `false` |
| `UseTracing` | Enable distributed tracing handler integration. | `bool`, default `false` |
| `MaxConnectionsPerServer` | Maximum concurrent connections per downstream server. | `int`, default `2147483647` |
| `PooledConnectionLifetimeSeconds` | Connection pool reuse duration in seconds. | `int`, default `120` |

---

## ServiceDiscoveryProvider

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `Type` | The service discovery provider implementation type. | `string`, default `"Consul"` |
| `Scheme` | Protocol for connecting to the discovery provider. | `string`, `"http"` or `"https"`, default `"http"` |
| `Host` | The service discovery server hostname or IP address. | `string`, default `"localhost"` |
| `Port` | The service discovery server port. | `int`, default `8500` (Consul) |
| `Token` | Authentication token for ACL-protected Consul instances. | `string`, optional |
| `ConfigurationKey` | The key for storing/retrieving config in Consul KV store. | `string`, default `"InternalConfiguration"` |
| `PollingInterval` | Polling frequency in milliseconds for `PollConsul`/`PollKube` provider updates. | `int`, optional |
| `Namespace` | Kubernetes namespace; not supported for Consul. | `string`, default `""` |

### Type 選項說明

| 值 | 官方說明 |
|----|---------|
| `Consul` | HashiCorp Consul — real-time query |
| `PollConsul` | Periodically polls Consul at `PollingInterval` |
| `Eureka` | Netflix Eureka service registry |
| `Kube` | Kubernetes — real-time API query |
| `PollKube` | Periodically polls Kubernetes API |
| `WatchKube` | Kubernetes Watch API for monitoring changes |
| `ServiceFabric` | Azure Service Fabric |
| Custom class name | Custom `IServiceDiscoveryProvider` implementation |

---

## MetadataOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `CurrentCulture` | Parsed as the `System.Globalization.CultureInfo` object for metadata value parsing. | `string`, default `CultureInfo.CurrentCulture.Name` |
| `NumberStyle` | Parsed as the `System.Globalization.NumberStyles` object for numeric metadata parsing. | `string`, default `"Any"` |
| `Separators` | Array of delimiter strings used to split metadata values. | `string[]`, default `[","]` |
| `StringSplitOption` | Parsed as the `System.StringSplitOptions` object. | `string`, default `"None"` |
| `TrimChars` | Array of characters to trim from parsed metadata values. | `char[]`, default `[" "]` |

### NumberStyle 選項

`None`, `AllowLeadingWhite`, `AllowTrailingWhite`, `AllowLeadingSign`, `AllowTrailingSign`, `AllowParentheses`, `AllowDecimalPoint`, `AllowThousands`, `AllowExponent`, `AllowCurrencySymbol`, `AllowHexSpecifier`, `Integer`, `HexNumber`, `Number`, `Float`, `Currency`, `Any`

### StringSplitOption 選項

`None`, `RemoveEmptyEntries`, `TrimEntries`

---

## Route Core Properties

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `UpstreamPathTemplate` | Specifies the URL that Ocelot uses to determine the appropriate DownstreamPathTemplate. Supports `{placeholder}` syntax. | `string`, **required** |
| `DownstreamPathTemplate` | Backend path pattern for request forwarding. | `string`, **required** |
| `DownstreamScheme` | Downstream protocol scheme. | `string`: `http`, `https`, `ws`, `wss` |
| `DownstreamHostAndPorts` | Specifies the host and port of downstream services. | `FileHostAndPort[]`, **required** |
| `UpstreamHttpMethod` | Enables Ocelot to differentiate between requests with different HTTP verbs. Leave empty to allow all methods. | `string[]` |
| `UpstreamHost` | Define routes based on the upstream host by examining the Host header. | `string` |
| `UpstreamHeaderTemplates` | Dictionary of headers that must match request headers for route matching. | `Dictionary<string, string>` |
| `DownstreamHttpMethod` | HTTP method override for downstream request (Method Transformation). | `string` |
| `DownstreamHttpVersion` | HTTP version for downstream connection. | `string`: `"1.0"`, `"1.1"`, `"2.0"` |
| `DownstreamHttpVersionPolicy` | HTTP version negotiation policy. | `string`: `RequestVersionExact`, `RequestVersionOrLower`, `RequestVersionOrHigher` |
| `Priority` | Defines the order in which routes match the upstream URL. 0 is lowest priority. | `int`, default `0` |
| `RouteIsCaseSensitive` | When true, enables case-sensitive URL matching. | `bool`, default `false` |
| `Key` | Route identifier for Aggregates and RateLimit references. | `string` |
| `RequestIdKey` | Request tracking identifier (Correlation ID) header name. | `string` |
| `ServiceName` | Service Discovery service name. | `string` |
| `ServiceNamespace` | Service Discovery service namespace. | `string` |
| `Timeout` | Request timeout in seconds, overrides global setting. | `int?`, default inherited |
| `DangerousAcceptAnyServerCertificateValidator` | Bypass SSL certificate validation. Not recommended for production. | `bool`, default `false` |

---

## SecurityOptions

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `IPAllowedList` | List of allowed IPs. Supports CIDR notation, IP ranges, and single IPs. | `string[]` |
| `IPBlockedList` | List of blocked IPs. Supports CIDR notation, IP ranges, and single IPs. | `string[]` |
| `ExcludeAllowedFromBlocked` | When true, excludes IPs in the allowed list from the blocked list. | `bool`, default `false` |

---

## Aggregates

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `UpstreamPathTemplate` | The route path that functions like a normal route. Must not duplicate Routes. | `string`, **required** |
| `RouteKeys` | Array of route keys to combine into a single response. | `string[]`, **required** |
| `UpstreamHost` | Optional hostname specification for the aggregate. | `string` |
| `RouteIsCaseSensitive` | Controls whether path matching is case-sensitive. | `bool` |
| `Aggregator` | Name of custom aggregator implementation. | `string` |
| `RouteKeysConfig` | Array enabling dynamic parameter mapping between routes. | `AggregateRouteConfig[]` |

### AggregateRouteConfig

| 屬性 | 官方說明 |
|------|---------|
| `RouteKey` | Reference identifier for the route. |
| `JsonPath` | JSONPath expression locating parameters in prior response. |
| `Parameter` | Parameter name to substitute in the route template. |

**Constraints:**
- Aggregation supports only the **GET** HTTP verb
- All downstream headers are discarded
- Responses return `application/json` content-type
- `RequestIdKey` is not supported in aggregates

---

## GlobalConfiguration Core

| 屬性 | 官方說明 | 值約束 |
|------|---------|--------|
| `BaseUrl` | Gateway public URL, used for header replacement and administration features. | `string` |
| `DownstreamScheme` | Global default downstream protocol. | `string` |
| `DownstreamHttpVersion` | Global default HTTP version. | `string`: `"1.0"`, `"1.1"`, `"2.0"` |
| `DownstreamHttpVersionPolicy` | Global default HTTP version policy. | `string` |
| `RequestIdKey` | Global default correlation ID header name. | `string` |
| `Timeout` | Global default request timeout in seconds. | `int?` |

> All sub-option objects in `GlobalConfiguration` (AuthenticationOptions, CacheOptions, QoSOptions, LoadBalancerOptions, HttpHandlerOptions, RateLimitOptions) additionally support a `RouteKeys` property: when set, only the specified routes receive the global defaults; empty/undefined = applies to all routes.
> Route-level settings **override** global configuration.
