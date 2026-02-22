# Ocelot 24.1 — ocelot.json 完整配置參考

> 來源：https://ocelot.readthedocs.io/en/latest/
> 版本：Ocelot **24.1**
> 整理日期：2026-02-13

---

## 目錄

1. [根結構 (Root Structure)](#1-根結構-root-structure)
2. [Routes — 路由配置](#2-routes--路由配置)
   - [核心屬性](#21-核心屬性)
   - [AuthenticationOptions](#22-authenticationoptions)
   - [RouteClaimsRequirement](#23-routeclaimsrequirement)
   - [CacheOptions](#24-cacheoptions)
   - [QoSOptions](#25-qosoptions)
   - [LoadBalancerOptions](#26-loadbalanceroptions)
   - [RateLimitOptions (Route)](#27-ratelimitoptions-route-level)
   - [HttpHandlerOptions](#28-httphandleroptions)
   - [SecurityOptions](#29-securityoptions)
   - [Header Transformation](#210-header-transformation)
   - [Claims Transformation](#211-claims-transformation)
   - [Method Transformation](#212-method-transformation)
   - [Metadata](#213-metadata)
   - [DelegatingHandlers](#214-delegatinghandlers)
   - [RateLimiting（新版）](#215-ratelimiting新版速率限制原始碼存在但未正式文件化)
3. [DynamicRoutes — 動態路由](#3-dynamicroutes--動態路由)
4. [Aggregates — 聚合路由](#4-aggregates--聚合路由)
5. [GlobalConfiguration — 全域配置](#5-globalconfiguration--全域配置)
   - [核心屬性](#51-核心屬性)
   - [ServiceDiscoveryProvider](#52-servicediscoveryprovider)
   - [MetadataOptions](#53-metadataoptions)
   - [RateLimitOptions (Global)](#54-ratelimitoptions-global-level)
6. [WebSocket 配置](#6-websocket-配置)
7. [Kubernetes (K8s) 配置](#7-kubernetes-k8s-配置)
8. [Service Fabric 配置](#8-service-fabric-配置)
9. [錯誤碼對照](#9-錯誤碼對照)
10. [完整 JSON 範例](#10-完整-json-範例)

---

## 1. 根結構 (Root Structure)

```json
{
  "Routes": [],
  "DynamicRoutes": [],
  "Aggregates": [],
  "GlobalConfiguration": {}
}
```

| 屬性 | 類型 | 必要 | 說明 |
|------|------|------|------|
| `Routes` | `FileRoute[]` | 是 | 靜態路由定義陣列 |
| `DynamicRoutes` | `FileDynamicRoute[]` | 否 | 動態路由定義陣列（搭配 Service Discovery） |
| `Aggregates` | `FileAggregateRoute[]` | 否 | 聚合路由定義陣列 |
| `GlobalConfiguration` | `FileGlobalConfiguration` | 否 | 全域預設配置 |

---

## 2. Routes — 路由配置

### 2.1 核心屬性

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `UpstreamPathTemplate` | `string` | **必填** | 上游（客戶端）請求路徑模板，支援 `{placeholder}` |
| `DownstreamPathTemplate` | `string` | **必填** | 下游（後端服務）路徑模板 |
| `DownstreamScheme` | `string` | **必填** | 下游協定：`http`、`https`、`ws`、`wss` |
| `DownstreamHostAndPorts` | `FileHostAndPort[]` | **必填** | 下游服務端點陣列 |
| `UpstreamHttpMethod` | `string[]` | 所有方法 | 允許的 HTTP 動詞，例如 `["Get", "Post"]` |
| `UpstreamHost` | `string` | `null` | 依 Host Header 匹配路由 |
| `UpstreamHeaderTemplates` | `Dictionary<string, string>` | `null` | 依 Header 匹配路由（支援 `{header:name}` 佔位符） |
| `DownstreamHttpMethod` | `string` | `null` | 轉換下游 HTTP 方法（Method Transformation） |
| `DownstreamHttpVersion` | `string` | `null` | 下游 HTTP 版本：`"1.0"`、`"1.1"`、`"2.0"` |
| `DownstreamHttpVersionPolicy` | `string` | `null` | HTTP 版本策略（見下方選項） |
| `Priority` | `int` | `0` | 路由匹配優先順序，數值越大越先匹配；`0` 為最低優先（Catch-All 路由固定為 `0`） |
| `RouteIsCaseSensitive` | `bool` | `false` | URL 匹配是否區分大小寫 |
| `Key` | `string` | `null` | 路由識別鍵（供 Aggregates 和 RateLimit 引用） |
| `RequestIdKey` | `string` | `null` | 關聯 ID（Correlation ID）Header 名稱 |
| `ServiceName` | `string` | `null` | Service Discovery 服務名稱 |
| `ServiceNamespace` | `string` | `null` | Service Discovery 服務命名空間 |
| `Timeout` | `int?` | `null` | 請求逾時（秒），覆蓋全域設定 |
| `DangerousAcceptAnyServerCertificateValidator` | `bool` | `false` | 跳過 SSL 憑證驗證（危險，僅開發用） |

#### DownstreamHttpVersionPolicy 選項

| 值 | 說明 |
|----|------|
| `RequestVersionExact` | 僅使用指定的 HTTP 版本 |
| `RequestVersionOrLower` | 使用指定版本或更低版本 |
| `RequestVersionOrHigher` | 使用指定版本或更高版本 |

#### FileHostAndPort

| 屬性 | 類型 | 說明 |
|------|------|------|
| `Host` | `string` | 主機名稱或 IP |
| `Port` | `int` | 埠號 |

---

### 2.2 AuthenticationOptions

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `AuthenticationProviderKeys` | `string[]` | `[]` | **建議使用** — 驗證提供者 Scheme 陣列，依序嘗試（First One Wins） |
| `AuthenticationProviderKey` | `string` | `""` | **已棄用 (v24.1)** — 單一驗證提供者 Scheme，v25.0 移除 |
| `AllowedScopes` | `string[]` | `[]` | 授權範圍限制，驗證 `scope` claim |
| `AllowAnonymous` | `bool?` | `null` | `true` = 排除此路由的全域驗證；`false` = 強制驗證 |

**行為說明：**
- 路由層級覆蓋全域設定
- 多個 Scheme 依陣列順序嘗試
- 驗證失敗回傳 **401 Unauthorized**

```json
"AuthenticationOptions": {
  "AuthenticationProviderKeys": ["Bearer", "MyKey"],
  "AllowedScopes": ["Admin", "User"],
  "AllowAnonymous": false
}
```

---

### 2.3 RouteClaimsRequirement

| 屬性 | 類型 | 說明 |
|------|------|------|
| `RouteClaimsRequirement` | `Dictionary<string, string>` | Claims 授權需求，Key=Claim Type, Value=Required Value |

- 驗證後（Post-Authentication）執行
- 不符合回傳 **403 Forbidden**

```json
"RouteClaimsRequirement": {
  "UserType": "registered",
  "Role": "Admin"
}
```

---

### 2.4 CacheOptions

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `TtlSeconds` | `int?` | `0` | 快取存活時間（秒），`0` = 停用快取 |
| `Region` | `string` | `""` | 快取區域名稱（供 Administration API 清除快取用） |
| `Header` | `string` | `"OC-Cache-Control"` | 原生快取控制 Header 名稱，納入快取鍵計算 |
| `EnableContentHashing` | `bool?` | `false` | 將請求 Body 雜湊納入快取鍵（適用於 POST/PUT） |

> `FileCacheOptions` 已棄用（v24.1+），請改用 `CacheOptions`。

```json
"CacheOptions": {
  "TtlSeconds": 15,
  "Region": "europe-central",
  "Header": "OC-Cache-Control",
  "EnableContentHashing": false
}
```

---

### 2.5 QoSOptions

| 屬性 | 類型 | 預設值 | 值約束 | 說明 |
|------|------|--------|--------|------|
| `Timeout` | `int` | `30000` | 10 < x < 86,400,000 ms | 請求逾時（毫秒） |
| `BreakDuration` | `int` | `5000` | 500 – 86,400,000 ms | 熔斷器開啟持續時間（毫秒） |
| `MinimumThroughput` | `int` | `100` | ≥ 2 | 取樣期間最低請求量才觸發熔斷判斷 |
| `FailureRatio` | `float` | `0.1` | > 0.0 且 ≤ 1.0 | 失敗/成功比率達此值即熔斷 |
| `SamplingDuration` | `int` | `30000` | 500 – 86,400,000 ms | 失敗率取樣視窗（毫秒） |

**已棄用屬性（v25.0 移除）：**
- `DurationOfBreak` → 改用 `BreakDuration`
- `ExceptionsAllowedBeforeBreaking` → 改用 `MinimumThroughput`
- `TimeoutValue` → 改用 `Timeout`

**Timeout 優先順序：**
1. QoS Timeout（毫秒，最高優先）
2. Route Timeout（秒）
3. Global Timeout（秒）
4. 預設 90 秒

```json
"QoSOptions": {
  "BreakDuration": 1000,
  "MinimumThroughput": 3,
  "FailureRatio": 0.5,
  "SamplingDuration": 10000,
  "Timeout": 5000
}
```

---

### 2.6 LoadBalancerOptions

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `Type` | `string` | — | 負載平衡類型（見下方選項） |
| `Key` | `string` | — | Cookie 名稱（僅 `CookieStickySessions` 使用） |
| `Expiry` | `int` | `1200000` | Session 過期時間（毫秒，僅 `CookieStickySessions`） |

#### Type 選項

| 值 | 說明 |
|----|------|
| `LeastConnection` | 將請求導向目前處理最少連線的服務 |
| `RoundRobin` | 依序輪流分配到各服務 |
| `CookieStickySessions` | 使用 Cookie 維持 Session 親和性，同一用戶導向同一服務 |
| `NoLoadBalancer` | 不負載平衡，選擇第一個可用服務 |

```json
"LoadBalancerOptions": {
  "Type": "CookieStickySessions",
  "Key": ".AspNetCore.Session",
  "Expiry": 1200000
}
```

---

### 2.7 RateLimitOptions (Route Level)

**當前屬性（v24.1）：**

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `EnableRateLimiting` | `bool?` | `true` | 啟用/停用速率限制 |
| `Limit` | `long?` | **必填** | Period 內最大請求數 |
| `Period` | `string` | **必填** | 限速視窗，時間單位：`ms`、`s`、`m`、`h`、`d` |
| `Wait` | `string` | `undefined` | 超額後停止服務時間，格式：`"1.5m"` |
| `ClientIdHeader` | `string` | `"Oc-Client"` | 識別客戶端的 Header 名稱 |
| `ClientWhitelist` | `string[]` | `[]` | 免受速率限制的客戶端清單 |
| `EnableHeaders` | `bool?` | `true` | 回傳 `X-RateLimit-*` 和 `Retry-After` Header |
| `StatusCode` | `int?` | `429` | 超額時的 HTTP 狀態碼 |
| `QuotaMessage` | `string` | 預設訊息 | 超額時的自訂回應訊息，支援 `{0}`=請求數 `{1}`=Period |
| `KeyPrefix` | `string` | `"Ocelot.RateLimiting"` | 快取計數器前綴 |

**已棄用屬性（v24.0 及更早，v25.0 移除）：**

| 已棄用 | 替代屬性 | 說明 |
|--------|----------|------|
| `DisableRateLimitHeaders` | `EnableHeaders` | 布林反轉；若兩者皆設定，棄用屬性優先 |
| `HttpStatusCode` | `StatusCode` | 超額時的 HTTP 狀態碼 |
| `QuotaExceededMessage` | `QuotaMessage` | 超額回應訊息 |
| `RateLimitCounterPrefix` | `KeyPrefix` | 快取鍵前綴 |
| `PeriodTimespan` | `Wait` | 超額後等待時間（秒） |

```json
"RateLimitOptions": {
  "EnableRateLimiting": true,
  "ClientWhitelist": ["api-key-1"],
  "Limit": 1000,
  "Period": "20s",
  "Wait": "1.5m",
  "StatusCode": 429,
  "QuotaMessage": "Quota exceeded: {0} requests per {1}"
}
```

> **注意：** 目前僅實作「By Client Header」分區規則。Sliding Window、Token Bucket、Concurrency 等演算法列於路線圖但尚未可用。

---

### 2.8 HttpHandlerOptions

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `AllowAutoRedirect` | `bool` | `false` | 是否自動跟隨 HTTP 3xx 重導向 |
| `UseCookieContainer` | `bool` | `false` | 是否在每個下游服務共享 Cookie |
| `UseProxy` | `bool` | `false` | 是否啟用 Proxy 支援 |
| `UseTracing` | `bool` | `false` | 是否啟用分散式追蹤（Tracing） |
| `MaxConnectionsPerServer` | `int` | `2147483647` | 每個伺服器最大並行連線數 |
| `PooledConnectionLifetimeSeconds` | `int` | `120` | 連線池連線存活時間（秒） |

```json
"HttpHandlerOptions": {
  "AllowAutoRedirect": true,
  "UseCookieContainer": false,
  "UseProxy": false,
  "UseTracing": true,
  "MaxConnectionsPerServer": 100,
  "PooledConnectionLifetimeSeconds": 120
}
```

---

### 2.9 SecurityOptions

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `IPAllowedList` | `string[]` | `null` | 允許的 IP 清單（支援 CIDR、範圍、單一 IP） |
| `IPBlockedList` | `string[]` | `null` | 封鎖的 IP 清單（支援 CIDR、範圍、單一 IP） |
| `ExcludeAllowedFromBlocked` | `bool` | `false` | 從封鎖清單中排除允許的 IP |

```json
"SecurityOptions": {
  "IPAllowedList": ["192.168.0.15", "10.0.0.0/24"],
  "IPBlockedList": ["192.168.0.0/23"],
  "ExcludeAllowedFromBlocked": true
}
```

---

### 2.10 Header Transformation

#### UpstreamHeaderTransform

**類型：** `Dictionary<string, string>`
**說明：** 在發送下游請求**之前**轉換 HTTP Header

#### DownstreamHeaderTransform

**類型：** `Dictionary<string, string>`
**說明：** 在收到下游回應**之後**轉換 HTTP Header

#### 轉換表達式類型

| 類型 | 語法 | 範例 |
|------|------|------|
| 常數值 | `"value"` | `"Bob"` |
| 查找替換 | `"find, replace"` | `"http://old.com/, http://new.com/"` |
| 佔位符 | `{placeholder}` | `{BaseUrl}`, `{DownstreamBaseUrl}`, `{RemoteIpAddress}`, `{TraceId}`, `{UpstreamHost}` |

#### 可用佔位符

| 佔位符 | 說明 |
|--------|------|
| `{BaseUrl}` | Ocelot 閘道器的 Base URL |
| `{DownstreamBaseUrl}` | 下游服務的 Base URL |
| `{RemoteIpAddress}` | 客戶端遠端 IP |
| `{TraceId}` | 追蹤 ID |
| `{UpstreamHost}` | 上游 Host Header |

```json
"UpstreamHeaderTransform": {
  "X-Forwarded-For": "{RemoteIpAddress}",
  "X-Custom": "StaticValue"
},
"DownstreamHeaderTransform": {
  "Location": "http://downstream.com/, http://gateway.com/",
  "X-Trace": "{TraceId}"
}
```

---

### 2.11 Claims Transformation

所有 Claims Transformation 屬性的值格式：

```
Claims[ClaimType] > value[index] > delimiter
```

- `Claims[...]`：指定 Claim 存取器
- `value`：直接使用 Claim 值
- `value[index]`：以 delimiter 分割後取第 index 個元素
- `>`：分隔符號

#### AddClaimsToRequest

**類型：** `Dictionary<string, string>`
**說明：** 將 Claims 轉換為新的 Claims（在 Authorization Middleware 之前執行）

```json
"AddClaimsToRequest": {
  "UserType": "Claims[sub] > value[0] > |",
  "UserId": "Claims[sub] > value[1] > |"
}
```

#### AddHeadersToRequest

**類型：** `Dictionary<string, string>`
**說明：** 將 Claims 轉換為 HTTP Header 傳送至下游

```json
"AddHeadersToRequest": {
  "CustomerId": "Claims[sub] > value[1] > |"
}
```

#### AddQueriesToRequest

**類型：** `Dictionary<string, string>`
**說明：** 將 Claims 轉換為 Query String 參數

```json
"AddQueriesToRequest": {
  "LocationId": "Claims[LocationId] > value"
}
```

#### ChangeDownstreamPathTemplate

**類型：** `Dictionary<string, string>`
**說明：** 將 Claim 值替換至 DownstreamPathTemplate 中的佔位符

```json
"DownstreamPathTemplate": "/api/users/{userId}/{everything}",
"ChangeDownstreamPathTemplate": {
  "userId": "Claims[sub] > value[1] > |"
}
```

---

### 2.12 Method Transformation

| 屬性 | 類型 | 說明 |
|------|------|------|
| `UpstreamHttpMethod` | `string[]` | 接受的上游 HTTP 方法 |
| `DownstreamHttpMethod` | `string` | 轉換後的下游 HTTP 方法 |

```json
{
  "UpstreamHttpMethod": ["Get"],
  "DownstreamHttpMethod": "Post"
}
```

---

### 2.13 Metadata

**類型：** `Dictionary<string, string>`
**說明：** 儲存任意鍵值對，可在 Middleware 或 DelegatingHandlers 中存取

```json
"Metadata": {
  "api-id": "FindPost",
  "tags": "tag1, tag2",
  "plugin1.enabled": "true",
  "plugin1.values": "[1, 2, 3, 4, 5]"
}
```

---

### 2.14 DelegatingHandlers

**類型：** `string[]`
**說明：** 自訂 Message Handler 類別名稱陣列，名稱必須與已註冊的 Handler 類別名稱一致

```json
"DelegatingHandlers": ["MyHandlerTwo", "MyHandler"]
```

---

### 2.15 RateLimiting（新版速率限制，原始碼存在但未正式文件化）

> **注意：** 此屬性存在於 Ocelot 24.1 原始碼 `FileRouteBase.RateLimiting` 中，但官方文件尚未完整記載。屬於路線圖功能，建議優先使用 `RateLimitOptions`。

| 屬性 | 類型 | 說明 |
|------|------|------|
| `RateLimiting` | `FileRateLimiting` | 新版速率限制配置（繼承 `FileRateLimitRule` 基礎屬性） |

#### FileRateLimiting 子屬性

| 屬性 | 類型 | 說明 |
|------|------|------|
| `ByHeader` | `FileRateLimitByHeaderRule` | 依客戶端 Header 限速（同 `RateLimitOptions`） |
| `ByMethod` | `FileGlobalRateLimit` | 依 HTTP 方法限速 |
| `ByIP` | `FileRateLimitByIpRule` | 依客戶端 IP 限速 |
| `ByAspNet` | `FileRateLimitByAspNetRule` | 使用 ASP.NET Core Rate Limiter 策略 |
| `Metadata` | `Dictionary<string, string>` | 自訂中繼資料 |

#### FileRateLimitByIpRule 子屬性

| 屬性 | 類型 | 說明 |
|------|------|------|
| `IPWhitelist` | `string[]` | 免受 IP 限速的白名單 |
| *(繼承 FileRateLimitRule 所有基礎屬性)* | | |

#### FileRateLimitByAspNetRule 子屬性

| 屬性 | 類型 | 說明 |
|------|------|------|
| `Policy` | `string` | ASP.NET Core Rate Limiter 策略名稱 |

---

## 3. DynamicRoutes — 動態路由

**說明：** 動態路由搭配 Service Discovery 使用，不需定義固定的 Path Template。

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `ServiceName` | `string` | — | Service Discovery 服務名稱 |
| `ServiceNamespace` | `string` | — | 服務命名空間 |
| `AuthenticationOptions` | `object` | — | 驗證配置 (v24.1+) |
| `CacheOptions` | `object` | — | 快取配置 (v24.1+) |
| `DownstreamHttpVersion` | `string` | — | `"1.0"`, `"1.1"`, `"2.0"` |
| `DownstreamHttpVersionPolicy` | `string` | — | HTTP 版本策略 |
| `HttpHandlerOptions` | `object` | — | Handler 配置 (v24.1+) |
| `LoadBalancerOptions` | `object` | — | 負載平衡配置 (v24.1+) |
| `Metadata` | `Dictionary<string, string>` | — | 自訂中繼資料 |
| `QoSOptions` | `object` | — | QoS 配置 (v24.1+) |
| `RateLimitOptions` | `object` | — | 速率限制 (v24.1+) |
| `Timeout` | `int?` | — | 逾時秒數 (v24.1+) |

> `RateLimitRule` 已棄用，請改用 `RateLimitOptions`。

---

## 4. Aggregates — 聚合路由

**說明：** 將多個路由的回應合併為單一回應。

| 屬性 | 類型 | 必要 | 說明 |
|------|------|------|------|
| `UpstreamPathTemplate` | `string` | 是 | 聚合端點路徑 |
| `UpstreamHttpMethod` | `string[]` | 否 | 允許的 HTTP 方法（僅支援 GET） |
| `UpstreamHost` | `string` | 否 | Host 過濾 |
| `RouteKeys` | `string[]` | 是 | 要聚合的路由 Key 陣列 |
| `RouteKeysConfig` | `AggregateRouteConfig[]` | 否 | 動態參數映射配置 |
| `Aggregator` | `string` | 否 | 自訂聚合器類別名稱 |
| `RouteIsCaseSensitive` | `bool` | 否 | 路徑大小寫敏感 |
| `Priority` | `int` | 否 | 匹配優先順序（預設 `0`） |
| `UpstreamHeaderTemplates` | `Dictionary<string, string>` | 否 | Header 匹配模板 |

#### AggregateRouteConfig (RouteKeysConfig)

| 屬性 | 類型 | 說明 |
|------|------|------|
| `RouteKey` | `string` | 引用路由的 Key 值 |
| `JsonPath` | `string` | JSONPath，指向第一個請求回應中的參數位置 |
| `Parameter` | `string` | 要替換的路由參數名稱 |

**限制：**
- `UpstreamPathTemplate` 不可與 Routes 重複
- 僅支援 **GET** 方法
- 回應 Header 在聚合過程中會遺失
- Content-Type 固定為 `application/json`
- 不支援 `RequestIdKey`

```json
{
  "Aggregates": [
    {
      "UpstreamPathTemplate": "/aggregated",
      "RouteKeys": ["Comments", "UserDetails"],
      "RouteKeysConfig": [
        {
          "RouteKey": "UserDetails",
          "JsonPath": "$[*].authorId",
          "Parameter": "userId"
        }
      ],
      "Aggregator": "MyCustomAggregator"
    }
  ]
}
```

---

## 5. GlobalConfiguration — 全域配置

### 5.1 核心屬性

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `BaseUrl` | `string` | — | Ocelot 閘道器的外部 URL（用於 Header 替換和管理功能） |
| `DownstreamScheme` | `string` | — | 全域預設下游協定 |
| `DownstreamHttpVersion` | `string` | — | 全域預設下游 HTTP 版本 |
| `DownstreamHttpVersionPolicy` | `string` | — | 全域預設 HTTP 版本策略（`RequestVersionExact`/`RequestVersionOrLower`/`RequestVersionOrHigher`） |
| `RequestIdKey` | `string` | — | 全域預設關聯 ID Header 名稱 |
| `Timeout` | `int?` | — | 全域預設逾時（秒） |
| `AuthenticationOptions` | `object` | — | 全域預設驗證配置 |
| `CacheOptions` | `object` | — | 全域預設快取配置 |
| `QoSOptions` | `object` | — | 全域預設 QoS 配置 |
| `LoadBalancerOptions` | `object` | — | 全域預設負載平衡配置 |
| `RateLimitOptions` | `object` | — | 全域預設速率限制配置 |
| `HttpHandlerOptions` | `object` | — | 全域預設 Handler 配置 |
| `SecurityOptions` | `object` | — | 全域預設安全配置 |
| `DownstreamHeaderTransform` | `Dictionary<string, string>` | — | 全域下游 Header 轉換（僅靜態路由） |
| `UpstreamHeaderTransform` | `Dictionary<string, string>` | — | 全域上游 Header 轉換（僅靜態路由） |
| `Metadata` | `Dictionary<string, string>` | — | 全域中繼資料（路由層級覆蓋） |
| `ServiceDiscoveryProvider` | `object` | — | Service Discovery 配置 |
| `MetadataOptions` | `object` | — | 中繼資料解析配置 |

> 路由層級設定**覆蓋**全域配置。

#### 全域子選項的 RouteKeys

所有 `GlobalConfiguration` 下的子選項物件（`AuthenticationOptions`、`CacheOptions`、`QoSOptions`、`LoadBalancerOptions`、`HttpHandlerOptions`、`RateLimitOptions`）皆額外支援 `RouteKeys` 屬性：

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `RouteKeys` | `string[]` | `undefined` | 指定套用的路由 Key 陣列；空或未定義 = 套用至所有路由 |

```json
"GlobalConfiguration": {
  "AuthenticationOptions": {
    "RouteKeys": ["SecureRoute1", "SecureRoute2"],
    "AuthenticationProviderKeys": ["Bearer"]
  },
  "CacheOptions": {
    "RouteKeys": ["CachedRoute"],
    "TtlSeconds": 300
  }
}
```

---

### 5.2 ServiceDiscoveryProvider

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `Type` | `string` | `"Consul"` | 提供者類型（見下方選項） |
| `Scheme` | `string` | `"http"` | 連線協定 |
| `Host` | `string` | `"localhost"` | 服務發現伺服器主機 |
| `Port` | `int` | 依提供者 | 服務發現伺服器埠號 |
| `Token` | `string` | `null` | ACL Token（Consul 認證用） |
| `ConfigurationKey` | `string` | `"InternalConfiguration"` | Consul KV Store 配置鍵 |
| `Namespace` | `string` | `""` | 命名空間（Kubernetes 使用） |
| `PollingInterval` | `int` | `null` | 輪詢間隔（毫秒，僅 Poll 類型） |

#### Type 選項

| 值 | 說明 |
|----|------|
| `Consul` | HashiCorp Consul |
| `PollConsul` | 定期輪詢 Consul |
| `Eureka` | Netflix Eureka |
| `Kube` | Kubernetes（即時查詢） |
| `PollKube` | 定期輪詢 Kubernetes |
| `WatchKube` | Kubernetes Watch 監控 |
| `ServiceFabric` | Azure Service Fabric |
| `自訂類別名稱` | 自訂 Service Discovery Provider |

```json
"ServiceDiscoveryProvider": {
  "Scheme": "https",
  "Host": "localhost",
  "Port": 8500,
  "Type": "Consul",
  "Token": "my-consul-token",
  "PollingInterval": 5000
}
```

---

### 5.3 MetadataOptions

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `CurrentCulture` | `string` | `CultureInfo.CurrentCulture.Name` | 解析用文化資訊（`CultureInfo`） |
| `NumberStyle` | `string` | `"Any"` | 數字解析樣式（`NumberStyles` 列舉） |
| `Separators` | `string[]` | `[","]` | 字串分割分隔符號陣列 |
| `StringSplitOption` | `string` | `"None"` | 字串分割選項（`StringSplitOptions` 列舉） |
| `TrimChars` | `char[]` | `[" "]` | 修剪字元陣列 |

#### NumberStyle 選項

`None`, `AllowLeadingWhite`, `AllowTrailingWhite`, `AllowLeadingSign`, `AllowTrailingSign`, `AllowParentheses`, `AllowDecimalPoint`, `AllowThousands`, `AllowExponent`, `AllowCurrencySymbol`, `AllowHexSpecifier`, `Integer`, `HexNumber`, `Number`, `Float`, `Currency`, `Any`

#### StringSplitOption 選項

`None`, `RemoveEmptyEntries`, `TrimEntries`

```json
"MetadataOptions": {
  "CurrentCulture": "en-GB",
  "NumberStyle": "Any",
  "Separators": [","],
  "StringSplitOption": "TrimEntries",
  "TrimChars": [" "]
}
```

---

### 5.4 RateLimitOptions (Global Level)

全域 RateLimitOptions 包含路由層級的所有屬性，加上 `RouteKeys`（見上方「全域子選項的 RouteKeys」章節）。

```json
"GlobalConfiguration": {
  "RateLimitOptions": {
    "RouteKeys": ["Route1", "Route2"],
    "ClientIdHeader": "Oc-Client",
    "Limit": 100,
    "Period": "30s",
    "Wait": "1m",
    "StatusCode": 429,
    "QuotaMessage": "Ocelot API calls quota exceeded!...",
    "KeyPrefix": "ocelot-rate-limiting"
  }
}
```

---

## 6. WebSocket 配置

WebSocket 路由使用標準路由結構，差異在 `DownstreamScheme`：

| Scheme | 說明 |
|--------|------|
| `ws` | 標準 WebSocket |
| `wss` | 加密 WebSocket (TLS) |

```json
{
  "UpstreamPathTemplate": "/ws",
  "DownstreamPathTemplate": "/ws",
  "DownstreamScheme": "wss",
  "DownstreamHostAndPorts": [
    { "Host": "localhost", "Port": 5001 }
  ]
}
```

---

## 7. Kubernetes (K8s) 配置

### 路由層級

| 屬性 | 類型 | 說明 |
|------|------|------|
| `ServiceName` | `string` | K8s 服務名稱 |
| `ServiceNamespace` | `string` | 覆蓋預設命名空間 |
| `DownstreamScheme` | `string` | 多埠服務的埠名稱選擇器 |

### GlobalConfiguration

```json
"ServiceDiscoveryProvider": {
  "Type": "Kube",
  "Scheme": "https",
  "Host": "my-k8s-host",
  "Port": 443,
  "Token": "my-bearer-token",
  "Namespace": "Dev"
}
```

#### K8s Type 選項

| 值 | 說明 |
|----|------|
| `Kube` | 即時查詢 K8s API |
| `PollKube` | 定期輪詢（需設定 `PollingInterval`） |
| `WatchKube` | 使用 K8s Watch API 監控變更 |

> 若 Pod 內使用 Service Account，`Scheme`、`Host`、`Port`、`Token` 會被忽略。

---

## 8. Service Fabric 配置

### 路由層級

| 屬性 | 類型 | 說明 |
|------|------|------|
| `ServiceName` | `string` | `應用程式名稱/服務名稱` 格式，支援 `{placeholder}` |

### GlobalConfiguration

```json
"ServiceDiscoveryProvider": {
  "Type": "ServiceFabric",
  "Host": "localhost",
  "Port": 19081
}
```

### 客戶端查詢參數（Stateful / Actor 服務）

| 參數 | 說明 |
|------|------|
| `PartitionKind` | 分區類型識別碼 |
| `PartitionKey` | 分區鍵值 |

---

## 9. 錯誤碼對照

### 客戶端錯誤 (4xx)

| 狀態碼 | 觸發條件 |
|--------|----------|
| **401 Unauthorized** | Authentication Middleware 驗證失敗 |
| **403 Forbidden** | Authorization 失敗、缺少必要 Claims/Scopes |
| **404 Not Found** | 下游路由不可用或內部錯誤碼無法對應 |
| **499 Client Closed Request** | 客戶端中斷連線（記錄 Warning） |

### 伺服器錯誤 (5xx)

| 狀態碼 | 觸發條件 |
|--------|----------|
| **500 Internal Server Error** | HTTP 請求失敗（非逾時、非取消的例外） |
| **502 Bad Gateway** | 無法連線至下游服務 |
| **503 Service Unavailable** | 下游請求逾時 |

---

## 10. 完整 JSON 範例

```json
{
  "Routes": [
    {
      "UpstreamPathTemplate": "/api/posts/{postId}",
      "UpstreamHttpMethod": ["Get", "Post"],
      "UpstreamHost": "mydomain.com",
      "UpstreamHeaderTemplates": {
        "version": "{header:versionnumber}"
      },
      "DownstreamPathTemplate": "/service/posts/{postId}",
      "DownstreamScheme": "https",
      "DownstreamHostAndPorts": [
        { "Host": "post-service", "Port": 5001 },
        { "Host": "post-service-2", "Port": 5002 }
      ],
      "DownstreamHttpVersion": "2.0",
      "Key": "PostRoute",
      "Priority": 0,
      "RouteIsCaseSensitive": false,
      "Timeout": 30,
      "RequestIdKey": "X-Request-Id",
      "DangerousAcceptAnyServerCertificateValidator": false,
      "AuthenticationOptions": {
        "AuthenticationProviderKeys": ["Bearer"],
        "AllowedScopes": ["read", "write"]
      },
      "RouteClaimsRequirement": {
        "Role": "Admin"
      },
      "AddClaimsToRequest": {
        "UserId": "Claims[sub] > value[1] > |"
      },
      "AddHeadersToRequest": {
        "X-User-Id": "Claims[sub] > value[1] > |"
      },
      "AddQueriesToRequest": {
        "userId": "Claims[sub] > value[1] > |"
      },
      "ChangeDownstreamPathTemplate": {},
      "UpstreamHeaderTransform": {
        "X-Forwarded-For": "{RemoteIpAddress}"
      },
      "DownstreamHeaderTransform": {
        "Location": "http://downstream.com/, http://gateway.com/"
      },
      "CacheOptions": {
        "TtlSeconds": 60,
        "Region": "posts",
        "Header": "OC-Cache-Control",
        "EnableContentHashing": false
      },
      "QoSOptions": {
        "Timeout": 5000,
        "BreakDuration": 3000,
        "MinimumThroughput": 10,
        "FailureRatio": 0.5,
        "SamplingDuration": 15000
      },
      "LoadBalancerOptions": {
        "Type": "LeastConnection"
      },
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Limit": 100,
        "Period": "1m",
        "ClientWhitelist": ["internal-service"]
      },
      "HttpHandlerOptions": {
        "AllowAutoRedirect": false,
        "UseCookieContainer": false,
        "UseProxy": false,
        "UseTracing": true,
        "MaxConnectionsPerServer": 200,
        "PooledConnectionLifetimeSeconds": 120
      },
      "SecurityOptions": {
        "IPAllowedList": ["10.0.0.0/8"],
        "IPBlockedList": ["192.168.1.100"],
        "ExcludeAllowedFromBlocked": true
      },
      "DelegatingHandlers": ["LoggingHandler"],
      "Metadata": {
        "api-id": "FindPost",
        "tags": "posts, content"
      }
    }
  ],
  "DynamicRoutes": [
    {
      "ServiceName": "dynamic-service",
      "LoadBalancerOptions": {
        "Type": "RoundRobin"
      },
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Limit": 50,
        "Period": "1m"
      }
    }
  ],
  "Aggregates": [
    {
      "UpstreamPathTemplate": "/api/dashboard",
      "RouteKeys": ["PostRoute", "UserRoute"],
      "RouteKeysConfig": [
        {
          "RouteKey": "UserRoute",
          "JsonPath": "$[*].authorId",
          "Parameter": "userId"
        }
      ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "https://api.example.com",
    "DownstreamScheme": "http",
    "DownstreamHttpVersion": "1.1",
    "RequestIdKey": "X-Request-Id",
    "Timeout": 60,
    "AuthenticationOptions": {
      "AuthenticationProviderKeys": ["Bearer"]
    },
    "CacheOptions": {
      "TtlSeconds": 300
    },
    "QoSOptions": {
      "Timeout": 30000,
      "BreakDuration": 5000,
      "MinimumThroughput": 100,
      "FailureRatio": 0.1,
      "SamplingDuration": 30000
    },
    "LoadBalancerOptions": {
      "Type": "LeastConnection"
    },
    "RateLimitOptions": {
      "ClientIdHeader": "Oc-Client",
      "StatusCode": 429
    },
    "HttpHandlerOptions": {
      "MaxConnectionsPerServer": 100,
      "PooledConnectionLifetimeSeconds": 120
    },
    "SecurityOptions": {
      "IPBlockedList": []
    },
    "ServiceDiscoveryProvider": {
      "Type": "Consul",
      "Scheme": "http",
      "Host": "localhost",
      "Port": 8500,
      "Token": "my-consul-token",
      "ConfigurationKey": "InternalConfiguration",
      "PollingInterval": 5000
    },
    "MetadataOptions": {
      "CurrentCulture": "en-US",
      "NumberStyle": "Any",
      "Separators": [","],
      "StringSplitOption": "TrimEntries",
      "TrimChars": [" "]
    },
    "Metadata": {
      "instance_name": "gateway-1"
    }
  }
}
```
