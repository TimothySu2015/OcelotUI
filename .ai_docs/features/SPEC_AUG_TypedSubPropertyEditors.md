# SPEC_AUG_TypedSubPropertyEditors

> 基於：SPEC_OcelotRouteEditor（已完成）
> 建立時間：2026-02-13
> 架構師：AI Architect
> 狀態：✅ 已完成
> Ocelot 版本對齊：**24.1**

## 疊加描述

將 Route 編輯器中 8 個以 `JsonElement?` 儲存的子物件（Authentication、RateLimit、LoadBalancer、QoS、Cache、HttpHandler、Security、Authorization），替換為**強型別 Domain Entity + 專用 UI 編輯元件**，讓使用者透過表單控制項（輸入框、下拉選單、Checkbox、清單編輯器）直接編輯子屬性，不再手寫 JSON。

同時：
- 為 Route 上現有的 `Dictionary<string,string>` 欄位（Metadata、Headers、Claims 等）和 `List<string>` 欄位（DelegatingHandlers）提供 UI 編輯元件
- 擴充 `OcelotGlobalConfiguration` 為完整強型別模型，並新增 GlobalConfig 編輯頁面
- 新增 `Aggregates`（路由聚合）和 `DynamicRoutes`（動態路由）的 Domain Model 與編輯 UI

---

## Phase A：Domain Layer — 子物件強型別化

### A1. 新增 Entity 類別

所有新類別放在 `src/Domain/Entities/`，遵循：
- Nullable Reference Types 啟用
- 所有屬性可為 null（表示未設定）
- 每個類別加 `[JsonExtensionData] Dictionary<string, JsonElement>? ExtensionData` 保護未知欄位

#### `AuthenticationOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| AllowAnonymous | bool? | — | 排除此路由不受全域認證影響 |
| AllowedScopes | List\<string\>? | — | 允許的 OAuth Scopes（至少一個即通過） |
| AuthenticationProviderKey | string? | — | 單一認證 Provider（v24.1 已棄用） |
| AuthenticationProviderKeys | List\<string\>? | — | 多認證 Provider，First One Wins 策略 |

#### `RateLimitOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| EnableRateLimiting | bool? | true | 啟用速率限制 |
| ClientIdHeader | string? | "Oc-Client" | 識別客戶端的 Header 名稱 |
| ClientWhitelist | List\<string\>? | — | 白名單客戶端 ID |
| EnableHeaders | bool? | true | 回應中加入速率限制相關 Headers |
| Limit | int? | — | 限制次數（必填啟用後） |
| Period | string? | — | 限制週期 (1s, 5m, 1h, 1d) |
| Wait | string? | — | 超限後等待時間 |
| StatusCode | int? | 429 | 超限回傳 HTTP 狀態碼 |
| QuotaMessage | string? | — | 超限回傳訊息（支援 {0},{1} 格式化） |
| KeyPrefix | string? | "Ocelot.RateLimiting" | 速率計數器 Key 前綴 |

#### `LoadBalancerOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| Type | string? | — | 負載均衡演算法 |
| Key | string? | — | Cookie Sticky Session 的 Cookie 名稱 |
| Expiry | int? | 1200000 | Sticky Session 過期時間（ms） |

**Type 有效值**：`RoundRobin`, `LeastConnection`, `CookieStickySessions`, `NoLoadBalancer`

#### `QoSOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| BreakDuration | int? | 5000 | 斷路器開啟持續時間（ms） |
| MinimumThroughput | int? | 100 | 取樣期間內最小請求數 |
| FailureRatio | double? | 0.1 | 失敗率閾值 (0.0~1.0) |
| SamplingDuration | int? | 30000 | 取樣時間窗口（ms） |
| Timeout | int? | 30000 | 請求逾時（ms） |

#### `CacheOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| TtlSeconds | int? | 0 | 快取存活時間（秒） |
| Region | string? | — | 快取區域識別（用於 Admin API 清除） |
| Header | string? | "OC-Cache-Control" | 快取控制 Header 名稱 |
| EnableContentHashing | bool? | false | 將 Request Body 雜湊納入快取鍵 |

#### `HttpHandlerOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| AllowAutoRedirect | bool? | false | 是否追蹤 3xx 重導向 |
| MaxConnectionsPerServer | int? | int.MaxValue | 每伺服器最大連線數 |
| PooledConnectionLifetimeSeconds | int? | 120 | 連線池中連線存活時間（秒） |
| UseCookieContainer | bool? | false | 是否保留 Cookie |
| UseProxy | bool? | false | 是否使用代理 |
| UseTracing | bool? | false | 是否啟用追蹤 |

#### `SecurityOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| IPAllowedList | List\<string\>? | — | 允許的 IP 清單（支援 CIDR） |
| IPBlockedList | List\<string\>? | — | 封鎖的 IP 清單（支援 CIDR） |
| ExcludeAllowedFromBlocked | bool? | false | 是否從封鎖清單中排除允許清單 |

#### `ServiceDiscoveryProvider.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| Type | string? | "Consul" | 服務發現類型 |
| Host | string? | "localhost" | 服務發現伺服器位址 |
| Port | int? | 8500 | 服務發現伺服器埠號 |
| Scheme | string? | "http" | 通訊協定 |
| Token | string? | — | ACL Token（Consul） |
| ConfigurationKey | string? | — | KV Store 設定鍵 |
| PollingInterval | int? | — | 輪詢間隔（ms） |
| Namespace | string? | — | Kubernetes 命名空間 |

#### `MetadataOptions.cs`

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| CurrentCulture | string? | — | CultureInfo 字串 |
| NumberStyle | string? | "Any" | NumberStyles 列舉字串 |
| Separators | List\<string\>? | [","] | 分隔字元 |
| StringSplitOption | string? | "None" | StringSplitOptions 列舉字串 |
| TrimChars | List\<string\>? | [" "] | 修剪字元 |
| Metadata | Dictionary\<string, string\>? | — | 全域 Metadata |

#### `OcelotAggregate.cs`

| 屬性 | 型別 | 說明 |
|------|------|------|
| UpstreamPathTemplate | string | 聚合路由的上游路徑 |
| RouteKeys | List\<string\> | 參與聚合的路由 Key 清單 |
| UpstreamHost | string? | 上游 Host 篩選 |
| RouteIsCaseSensitive | bool? | 路徑大小寫敏感 |
| Aggregator | string? | 自訂聚合器類別名稱 |
| RouteKeysConfig | List\<AggregateRouteConfig\>? | 進階聚合設定 |

#### `AggregateRouteConfig.cs`

| 屬性 | 型別 | 說明 |
|------|------|------|
| RouteKey | string | 路由 Key |
| JsonPath | string? | JSONPath 表達式 |
| Parameter | string? | 目標參數名稱 |

#### `OcelotDynamicRoute.cs`

| 屬性 | 型別 | 說明 |
|------|------|------|
| ServiceName | string? | 服務名稱 |
| ServiceNamespace | string? | 服務命名空間 |
| DownstreamHttpVersion | string? | HTTP 版本 |
| DownstreamHttpVersionPolicy | string? | 版本政策 |
| Timeout | int? | 逾時（秒） |
| Metadata | Dictionary\<string, string\>? | 自訂 Metadata |
| AuthenticationOptions | AuthenticationOptions? | 認證設定 |
| RateLimitOptions | RateLimitOptions? | 速率限制 |
| LoadBalancerOptions | LoadBalancerOptions? | 負載均衡 |
| QoSOptions | QoSOptions? | 服務品質 |
| CacheOptions | CacheOptions? | 快取 |
| HttpHandlerOptions | HttpHandlerOptions? | HTTP Handler |

### A2. 修改現有 Entity

#### `OcelotRoute.cs` 變更

將下列屬性從 `JsonElement?` 改為強型別：

```
- JsonElement? AuthenticationOptions  →  AuthenticationOptions? AuthenticationOptions
- JsonElement? AuthorizationOptions   →  移除（已由 RouteClaimsRequirement Dict 涵蓋）
- JsonElement? RateLimitOptions       →  RateLimitOptions? RateLimitOptions
- JsonElement? LoadBalancerOptions    →  LoadBalancerOptions? LoadBalancerOptions
- JsonElement? QoSOptions             →  QoSOptions? QoSOptions
- JsonElement? CacheOptions           →  CacheOptions? CacheOptions
- JsonElement? HttpHandlerOptions     →  HttpHandlerOptions? HttpHandlerOptions
- JsonElement? SecurityOptions        →  SecurityOptions? SecurityOptions
```

#### `OcelotGlobalConfiguration.cs` 全面擴充

```csharp
public class OcelotGlobalConfiguration
{
    // 基本設定
    public string? BaseUrl { get; set; }
    public string? DownstreamScheme { get; set; }
    public string? DownstreamHttpVersion { get; set; }
    public string? DownstreamHttpVersionPolicy { get; set; }
    public string? RequestIdKey { get; set; }
    public int? Timeout { get; set; }

    // 子物件（與 Route 共用型別）
    public AuthenticationOptions? AuthenticationOptions { get; set; }
    public RateLimitOptions? RateLimitOptions { get; set; }
    public LoadBalancerOptions? LoadBalancerOptions { get; set; }
    public QoSOptions? QoSOptions { get; set; }
    public CacheOptions? CacheOptions { get; set; }
    public HttpHandlerOptions? HttpHandlerOptions { get; set; }
    public SecurityOptions? SecurityOptions { get; set; }

    // 全域專屬
    public ServiceDiscoveryProvider? ServiceDiscoveryProvider { get; set; }
    public MetadataOptions? MetadataOptions { get; set; }

    // Dictionary 欄位
    public Dictionary<string, string>? Metadata { get; set; }
    public Dictionary<string, string>? UpstreamHeaderTransform { get; set; }
    public Dictionary<string, string>? DownstreamHeaderTransform { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
```

#### `OcelotConfiguration.cs` 新增欄位

```csharp
public class OcelotConfiguration
{
    public List<OcelotRoute> Routes { get; set; } = [];
    public List<OcelotDynamicRoute>? DynamicRoutes { get; set; }
    public List<OcelotAggregate>? Aggregates { get; set; }
    public OcelotGlobalConfiguration? GlobalConfiguration { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
```

---

## Phase B：共用 UI 元件

放在 `src/Web/Components/Shared/`

### B1. `StringListEditor.razor`

**用途**：編輯 `List<string>` 類型欄位（AllowedScopes、IPAllowedList、ClientWhitelist、DelegatingHandlers 等）

**介面**：
```razor
<StringListEditor @bind-Items="model.AllowedScopes"
                  Label="Allowed Scopes"
                  Placeholder="e.g. api.read" />
```

**UI 行為**：
- 顯示目前清單中所有項目，每項一行
- 每項右側有刪除按鈕（IconButton × ）
- 底部有一列：MudTextField + 「新增」按鈕
- 空清單時顯示提示文字

**控制項**：
| 元素 | MudBlazor 元件 |
|------|----------------|
| 項目文字 | MudChip 或 MudText |
| 刪除 | MudIconButton (Delete) |
| 新增輸入 | MudTextField + MudIconButton (Add) |

### B2. `DictionaryEditor.razor`

**用途**：編輯 `Dictionary<string, string>` 類型欄位（Metadata、AddHeadersToRequest、RouteClaimsRequirement 等）

**介面**：
```razor
<DictionaryEditor @bind-Items="model.Metadata"
                  KeyLabel="Key"
                  ValueLabel="Value"
                  KeyPlaceholder="e.g. project"
                  ValuePlaceholder="e.g. ocelot-ui" />
```

**UI 行為**：
- 以表格形式顯示所有 Key-Value 對
- 每行：Key (readonly text) | Value (readonly text) | 刪除按鈕
- 底部新增列：Key 輸入 | Value 輸入 | 新增按鈕
- 防止重複 Key

**控制項**：
| 元素 | MudBlazor 元件 |
|------|----------------|
| Key/Value 顯示 | MudText |
| 刪除 | MudIconButton (Delete) |
| Key 輸入 | MudTextField |
| Value 輸入 | MudTextField |
| 新增 | MudIconButton (Add) |

### B3. `AggregateRouteConfigEditor.razor`

**用途**：編輯 `List<AggregateRouteConfig>` (RouteKeysConfig)

**UI 行為**：
- 每筆顯示 RouteKey、JsonPath、Parameter 三個欄位
- 新增 / 刪除行

---

## Phase C：子物件編輯元件

放在 `src/Web/Components/Shared/`，每個元件以 `MudStack` 排列欄位。

### C1. `AuthenticationOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| AllowAnonymous | MudCheckBox\<bool?\> | |
| AuthenticationProviderKeys | StringListEditor | 主要使用 |
| AuthenticationProviderKey | MudTextField (disabled 樣式 + 標註 Deprecated) | v24.1 已棄用 |
| AllowedScopes | StringListEditor | |

### C2. `RateLimitOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| EnableRateLimiting | MudCheckBox\<bool?\> | |
| Limit | MudNumericField\<int?\> | |
| Period | MudTextField + HelperText 顯示格式 | 格式：1s, 5m, 1h, 1d |
| Wait | MudTextField | 同 Period 格式 |
| ClientIdHeader | MudTextField | |
| ClientWhitelist | StringListEditor | |
| EnableHeaders | MudCheckBox\<bool?\> | |
| StatusCode | MudNumericField\<int?\> | |
| QuotaMessage | MudTextField | |
| KeyPrefix | MudTextField | |

### C3. `LoadBalancerOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| Type | MudSelect\<string?\> | 選項：RoundRobin, LeastConnection, CookieStickySessions, NoLoadBalancer |
| Key | MudTextField | 僅 CookieStickySessions 時顯示 |
| Expiry | MudNumericField\<int?\> | 僅 CookieStickySessions 時顯示，單位 ms |

**條件顯示**：當 `Type == "CookieStickySessions"` 時才顯示 Key 和 Expiry 欄位。

### C4. `QoSOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| BreakDuration | MudNumericField\<int?\> | 單位 ms，>500 <86400000 |
| MinimumThroughput | MudNumericField\<int?\> | ≥2 |
| FailureRatio | MudNumericField\<double?\> | 0.0~1.0，Step=0.01 |
| SamplingDuration | MudNumericField\<int?\> | 單位 ms |
| Timeout | MudNumericField\<int?\> | 單位 ms |

### C5. `CacheOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| TtlSeconds | MudNumericField\<int?\> | 0 = 不快取 |
| Region | MudTextField | |
| Header | MudTextField | 預設 OC-Cache-Control |
| EnableContentHashing | MudCheckBox\<bool?\> | POST/PUT 時建議啟用 |

### C6. `HttpHandlerOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| AllowAutoRedirect | MudCheckBox\<bool?\> | |
| UseCookieContainer | MudCheckBox\<bool?\> | |
| UseProxy | MudCheckBox\<bool?\> | |
| UseTracing | MudCheckBox\<bool?\> | |
| MaxConnectionsPerServer | MudNumericField\<int?\> | |
| PooledConnectionLifetimeSeconds | MudNumericField\<int?\> | 單位秒 |

### C7. `SecurityOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| IPAllowedList | StringListEditor | 支援 CIDR、IP Range |
| IPBlockedList | StringListEditor | 支援 CIDR、IP Range |
| ExcludeAllowedFromBlocked | MudCheckBox\<bool?\> | |

### C8. `ServiceDiscoveryProviderEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| Type | MudSelect\<string?\> | Consul, PollConsul, Eureka |
| Host | MudTextField | |
| Port | MudNumericField\<int?\> | |
| Scheme | MudSelect\<string?\> | http, https |
| Token | MudTextField | |
| ConfigurationKey | MudTextField | |
| PollingInterval | MudNumericField\<int?\> | 單位 ms |
| Namespace | MudTextField | |

### C9. `MetadataOptionsEditor.razor`

| 欄位 | 控制項 | 備註 |
|------|--------|------|
| CurrentCulture | MudTextField | |
| NumberStyle | MudTextField | |
| Separators | StringListEditor | |
| StringSplitOption | MudTextField | |
| TrimChars | StringListEditor | |
| Metadata | DictionaryEditor | |

---

## Phase D：RouteEdit 頁面重構

### D1. 替換 Advanced JSON 區段

**移除**：`_advancedSections` 陣列、`GetJsonSection()`、`SetJsonSection()` 方法

**替換為**：在 `MudExpansionPanels` 內，每個子物件使用專用編輯元件：

```razor
<MudExpansionPanels>
    <MudExpansionPanel Text="@L["RouteEdit_Options"]">
        @* 現有 Options 欄位不變 *@
    </MudExpansionPanel>

    <MudExpansionPanel Text="@L["RouteEdit_AuthenticationOptions"]">
        <AuthenticationOptionsEditor @bind-Value="_route.AuthenticationOptions" />
    </MudExpansionPanel>

    <MudExpansionPanel Text="@L["RouteEdit_RateLimitOptions"]">
        <RateLimitOptionsEditor @bind-Value="_route.RateLimitOptions" />
    </MudExpansionPanel>

    @* ... 其他子物件 *@

    <MudExpansionPanel Text="@L["RouteEdit_Metadata"]">
        <DictionaryEditor @bind-Items="_route.Metadata" ... />
    </MudExpansionPanel>

    <MudExpansionPanel Text="@L["RouteEdit_AddHeadersToRequest"]">
        <DictionaryEditor @bind-Items="_route.AddHeadersToRequest" ... />
    </MudExpansionPanel>

    @* ... 其他 Dictionary 欄位 *@

    <MudExpansionPanel Text="@L["RouteEdit_DelegatingHandlers"]">
        <StringListEditor @bind-Items="_route.DelegatingHandlers" ... />
    </MudExpansionPanel>
</MudExpansionPanels>
```

### D2. 每個子編輯元件的 bind 模式

所有子編輯元件統一使用 `@bind-Value` 雙向綁定模式：

```razor
@* Component interface *@
[Parameter] public TOptions? Value { get; set; }
[Parameter] public EventCallback<TOptions?> ValueChanged { get; set; }
```

元件內部在任何欄位變更時：
1. 若所有欄位都為 null/預設值，則呼叫 `ValueChanged.InvokeAsync(null)` — 避免寫入空物件
2. 否則建構新的 `TOptions` 實例並呼叫 `ValueChanged.InvokeAsync(newValue)`

### D3. ApplyTemplate 更新

`ApplyTemplate` 方法需更新以對應新的強型別屬性。原有 `JsonElement?` 的賦值改為直接物件賦值（深拷貝）。`RouteTemplateProvider` 中的範本也需更新為使用強型別。

---

## Phase E：GlobalConfiguration 編輯頁面

### E1. Application Layer

新增 CQRS 結構：

```
Application/GlobalConfig/
├── Queries/
│   └── GetGlobalConfig/
│       ├── GetGlobalConfigQuery.cs
│       ├── GetGlobalConfigQueryHandler.cs
│       └── GlobalConfigDto.cs
└── Commands/
    └── UpdateGlobalConfig/
        ├── UpdateGlobalConfigCommand.cs
        └── UpdateGlobalConfigCommandHandler.cs
```

- `GetGlobalConfigQuery` → 回傳 `Result<GlobalConfigDto>`
- `GlobalConfigDto` 包含 `OcelotGlobalConfiguration` 實例
- `UpdateGlobalConfigCommand` 接收 `GlobalConfigDto`，寫回 ocelot.json

### E2. `GlobalConfigEdit.razor`

**路由**：`/global-config`

**版面配置**（與 RouteEdit 相同風格）：

1. **工具列**：儲存 / 取消按鈕
2. **基本設定區**（MudPaper）：
   - BaseUrl (MudTextField)
   - DownstreamScheme (MudSelect: https, http)
   - DownstreamHttpVersion (MudTextField)
   - DownstreamHttpVersionPolicy (MudTextField)
   - RequestIdKey (MudTextField)
   - Timeout (MudNumericField)
3. **服務發現**（MudExpansionPanel）：
   - ServiceDiscoveryProviderEditor
4. **子物件區段**（MudExpansionPanels）— 與 Route 共用元件：
   - AuthenticationOptionsEditor
   - RateLimitOptionsEditor
   - LoadBalancerOptionsEditor
   - QoSOptionsEditor
   - CacheOptionsEditor
   - HttpHandlerOptionsEditor
   - SecurityOptionsEditor
   - MetadataOptionsEditor
5. **Header Transform 區段**（MudExpansionPanels）：
   - DictionaryEditor: UpstreamHeaderTransform
   - DictionaryEditor: DownstreamHeaderTransform
   - DictionaryEditor: Metadata

### E3. NavMenu 更新

在 NavMenu 加入 GlobalConfig 連結（已有預留）。

---

## Phase F：Aggregates & DynamicRoutes 編輯 UI

### F1. Aggregates

**路由**：`/aggregates`（列表）、`/aggregates/new`、`/aggregates/edit/{index}`

**Application Layer**：
```
Application/Aggregates/
├── Queries/
│   ├── GetAllAggregates/
│   └── GetAggregateByIndex/
└── Commands/
    ├── AddAggregate/
    ├── UpdateAggregate/
    └── DeleteAggregate/
```

**AggregateEdit.razor** 版面：
1. UpstreamPathTemplate (MudTextField)
2. RouteKeys (StringListEditor) — 下拉選擇已存在的 Route Key
3. UpstreamHost (MudTextField)
4. RouteIsCaseSensitive (MudCheckBox)
5. Aggregator (MudTextField)
6. RouteKeysConfig (AggregateRouteConfigEditor)

### F2. DynamicRoutes

**路由**：`/dynamic-routes`（列表）、`/dynamic-routes/new`、`/dynamic-routes/edit/{index}`

**Application Layer**：
```
Application/DynamicRoutes/
├── Queries/
│   ├── GetAllDynamicRoutes/
│   └── GetDynamicRouteByIndex/
└── Commands/
    ├── AddDynamicRoute/
    ├── UpdateDynamicRoute/
    └── DeleteDynamicRoute/
```

**DynamicRouteEdit.razor** 版面：
1. ServiceName (MudTextField)
2. ServiceNamespace (MudTextField)
3. DownstreamHttpVersion / DownstreamHttpVersionPolicy
4. Timeout (MudNumericField)
5. 子物件區段（共用編輯元件）

---

## Phase G：i18n 資源

### 新增翻譯鍵（約 80+ 組）

分類：
- `Auth_*` — Authentication 編輯器欄位標籤
- `RateLimit_*` — Rate Limit 編輯器
- `LoadBalancer_*` — Load Balancer 編輯器
- `QoS_*` — QoS 編輯器
- `Cache_*` — Cache 編輯器
- `HttpHandler_*` — HTTP Handler 編輯器
- `Security_*` — Security 編輯器
- `ServiceDiscovery_*` — Service Discovery 編輯器
- `MetadataOpt_*` — Metadata Options 編輯器
- `GlobalConfig_*` — Global Config 頁面
- `Aggregate_*` — Aggregates 頁面
- `DynamicRoute_*` — Dynamic Routes 頁面
- `DictEditor_*` — Dictionary 編輯器
- `ListEditor_*` — String List 編輯器
- `Tip_*` — 各子編輯器的 Tooltip

---

## 影響分析

### 修改的檔案

| 檔案 | 變更 |
|------|------|
| `src/Domain/Entities/OcelotRoute.cs` | `JsonElement?` → 強型別 |
| `src/Domain/Entities/OcelotGlobalConfiguration.cs` | 擴充所有屬性 |
| `src/Domain/Entities/OcelotConfiguration.cs` | 新增 DynamicRoutes, Aggregates |
| `src/Web/Components/Pages/Routes/RouteEdit.razor` | 替換 JSON 編輯器為元件 |
| `src/Web/Services/RouteTemplateProvider.cs` | 範本改用強型別 |
| `src/Web/Components/Layout/NavMenu.razor` | 新增導航連結 |
| `src/Web/Resources/SharedResource.resx` | 新增翻譯鍵 |
| `src/Web/Resources/SharedResource.zh-TW.resx` | 新增翻譯鍵 |

### 新增的檔案

**Domain (11 個)**：
- `AuthenticationOptions.cs`
- `RateLimitOptions.cs`
- `LoadBalancerOptions.cs`
- `QoSOptions.cs`
- `CacheOptions.cs`
- `HttpHandlerOptions.cs`
- `SecurityOptions.cs`
- `ServiceDiscoveryProvider.cs`
- `MetadataOptions.cs`
- `OcelotAggregate.cs`, `AggregateRouteConfig.cs`
- `OcelotDynamicRoute.cs`

**Web Shared Components (12 個)**：
- `StringListEditor.razor`
- `DictionaryEditor.razor`
- `AuthenticationOptionsEditor.razor`
- `RateLimitOptionsEditor.razor`
- `LoadBalancerOptionsEditor.razor`
- `QoSOptionsEditor.razor`
- `CacheOptionsEditor.razor`
- `HttpHandlerOptionsEditor.razor`
- `SecurityOptionsEditor.razor`
- `ServiceDiscoveryProviderEditor.razor`
- `MetadataOptionsEditor.razor`
- `AggregateRouteConfigEditor.razor`

**Web Pages (5 個)**：
- `GlobalConfigEdit.razor`
- `AggregateList.razor`, `AggregateEdit.razor`
- `DynamicRouteList.razor`, `DynamicRouteEdit.razor`

**Application Layer (CQRS, 約 15 個)**：
- GlobalConfig: Query + Command (4 個)
- Aggregates: 2 Queries + 3 Commands (10 個)
- DynamicRoutes: 2 Queries + 3 Commands (10 個)

### 不需變更的檔案

- `src/Infrastructure/` — Repository 透過 Domain Model 自動支援新型別（System.Text.Json 序列化/反序列化不需改動）
- `src/Application/Routes/` — 現有 Route CQRS 不需改動（RouteDetailDto 直接使用 OcelotRoute）
- `src/Domain/Common/` — BaseEntity, IDomainEvent 不變

### Breaking Changes

- ⚠️ `OcelotRoute` 的 8 個 `JsonElement?` 屬性改為強型別類別
  - **影響**：RouteEdit.razor（已在 Phase D 處理）、RouteTemplateProvider（已在 Phase D3 處理）
  - **不影響**：JSON 序列化結果（輸出格式完全相同）
  - **不影響**：Infrastructure Repository（System.Text.Json 會自動處理）
- ⚠️ 移除 `AuthorizationOptions` (JsonElement?) — 功能由 `RouteClaimsRequirement` (Dictionary) 涵蓋

---

## 實作順序建議

```
Phase A (Domain)  ─── 先做，因為其他 Phase 都依賴
    │
    ├── Phase B (共用元件) ─── 第二，被 C/D/E/F 依賴
    │       │
    │       ├── Phase C (子物件編輯器)
    │       │       │
    │       │       ├── Phase D (RouteEdit 重構)
    │       │       └── Phase E (GlobalConfig 頁面)
    │       │               │
    │       │               └── Phase F (Aggregates & DynamicRoutes)
    │       │
    │       └── Phase G (i18n) ─── 可與 C~F 平行進行
```

---

## 驗證 Checklist

- [ ] `dotnet build` — 0 errors, 0 warnings
- [ ] 現有 ocelot.json 可正常讀取（向後相容）
- [ ] 儲存後的 ocelot.json 格式與原始格式一致（Round-trip 保真）
- [ ] null 子物件不寫入 JSON（`DefaultIgnoreCondition.WhenWritingNull`）
- [ ] 未知屬性透過 `[JsonExtensionData]` 保留
- [ ] 所有 UI 控制項可正常雙向綁定
- [ ] 模板套用功能正常（強型別）
- [ ] GlobalConfig 頁面可正常讀取/儲存
- [ ] Aggregates 頁面可正常 CRUD
- [ ] DynamicRoutes 頁面可正常 CRUD
- [ ] 中英文切換正常

---

✅ 疊加藍圖已完成，請確認後呼叫 `/coder AUG_TypedSubPropertyEditors` 開始實作
