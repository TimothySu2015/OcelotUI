# SPEC_AUG_FieldTooltips

> 基於：SPEC_OcelotRouteEditor.md + SPEC_AUG_I18nTemplatesGuide.md
> 建立時間：2026-02-13
> 架構師：AI Architect
> 狀態：🚧 實作中

## 疊加描述

在 RouteEdit 編輯器畫面的每個表單欄位上，增加 **Hover Tooltip** 功能說明。
使用者將滑鼠移至欄位上方時，會彈出該欄位的用途說明（支援 i18n 中英文切換）。

### 實作方式

使用 MudBlazor 的 `<MudTooltip>` 元件包裹每個表單欄位（MudTextField、MudSelect、MudNumericField、MudCheckBox、MudExpansionPanel 等）。
Tooltip 文字透過 `IStringLocalizer<SharedResource>` 取得，支援多語系。

---

## 影響分析

### 受影響的檔案

| 檔案 | 變更類型 | 說明 |
|------|----------|------|
| `src/Web/Components/Pages/Routes/RouteEdit.razor` | 修改 UI | 每個表單欄位外層包裹 `MudTooltip` |
| `src/Web/Components/Shared/DownstreamHostEditor.razor` | 修改 UI | Host / Port 欄位包裹 `MudTooltip` |
| `src/Web/Components/Shared/HttpMethodSelector.razor` | 修改 UI | 整個選擇器包裹 `MudTooltip` |
| `src/Web/Resources/SharedResource.resx` | 新增資源 | 新增 20 組 Tooltip 英文文字 |
| `src/Web/Resources/SharedResource.zh-TW.resx` | 新增資源 | 新增 20 組 Tooltip 繁中文字 |

### 不需變更的檔案

- Domain Layer — 無影響
- Application Layer — 無影響
- Infrastructure Layer — 無影響
- RouteList.razor — 列表頁無需 Tooltip
- GuidePage.razor — 教學頁無需 Tooltip
- Program.cs — 無需變更

### 不可破壞的規則

- ⚠️ 所有現有欄位的資料綁定 (`@bind-Value`) 必須維持正常
- ⚠️ 表單提交 (EditForm OnValidSubmit) 功能不可受影響
- ⚠️ 現有 i18n 鍵值不可修改
- ⚠️ RouteTemplate 套用功能不可受影響

---

## Tooltip 對照表

### Upstream Section

| # | 欄位 | i18n Key | EN 說明 | zh-TW 說明 |
|---|------|----------|---------|------------|
| 1 | Upstream Path Template | `Tip_UpstreamPath` | The URL pattern that Ocelot matches against incoming client requests. Supports placeholders like {id} and catch-all {everything}. | Ocelot 用來比對客戶端請求的 URL 模式。支援佔位符如 {id} 及萬用字元 {everything}。 |
| 2 | HTTP Methods | `Tip_HttpMethods` | Select which HTTP methods this route will match. Leave empty to match all methods. | 選擇此路由要比對的 HTTP 方法。留空則比對所有方法。 |
| 3 | Upstream Host | `Tip_UpstreamHost` | Optional host header filter. Only requests with this Host header will match this route. | 選填的 Host 標頭篩選。只有帶此 Host 標頭的請求才會匹配此路由。 |

### Downstream Section

| # | 欄位 | i18n Key | EN 說明 | zh-TW 說明 |
|---|------|----------|---------|------------|
| 4 | Downstream Path Template | `Tip_DownstreamPath` | The URL path that Ocelot forwards the request to on the downstream service. Use matching placeholders from upstream. | Ocelot 將請求轉發至下游服務的 URL 路徑。使用與上游相同的佔位符。 |
| 5 | Scheme | `Tip_Scheme` | The protocol used to communicate with the downstream service: https, http, ws (WebSocket), or wss (secure WebSocket). | 與下游服務通訊使用的協定：https、http、ws（WebSocket）或 wss（安全 WebSocket）。 |
| 6 | HTTP Version | `Tip_HttpVersion` | The HTTP version used for downstream requests. Common values: 1.1, 2.0. Leave empty for default. | 下游請求使用的 HTTP 版本。常用值：1.1、2.0。留空使用預設值。 |
| 7 | Downstream Host & Ports | `Tip_HostAndPorts` | The target server addresses. Add multiple hosts for load balancing. Each entry needs a hostname and port. | 目標伺服器位址。新增多個主機可實現負載均衡。每筆需填主機名稱與連接埠。 |

### Options Section

| # | 欄位 | i18n Key | EN 說明 | zh-TW 說明 |
|---|------|----------|---------|------------|
| 8 | Key | `Tip_Key` | A unique identifier for this route. Required when using service discovery or route aggregation. | 此路由的唯一識別碼。使用服務發現或路由聚合時為必填。 |
| 9 | Priority | `Tip_Priority` | Route matching priority. Lower numbers are matched first. Use when multiple routes could match the same request. | 路由比對優先順序。數字越小優先權越高。當多條路由可能匹配同一請求時使用。 |
| 10 | Timeout | `Tip_Timeout` | Maximum time (in seconds) to wait for a downstream response before timing out. | 等待下游回應的最長時間（秒）。超過此時間將逾時中斷。 |
| 11 | Case Sensitive | `Tip_CaseSensitive` | Whether URL path matching is case-sensitive. Default is false (case-insensitive). | URL 路徑比對是否區分大小寫。預設為否（不區分大小寫）。 |
| 12 | Accept Any Certificate | `Tip_AcceptAnyCert` | Accept any SSL/TLS certificate from downstream, including self-signed. Use only for development. | 接受下游的任何 SSL/TLS 憑證，包括自簽憑證。僅建議在開發環境使用。 |

### Advanced JSON Sections

| # | 欄位 | i18n Key | EN 說明 | zh-TW 說明 |
|---|------|----------|---------|------------|
| 13 | Authentication Options | `Tip_AuthenticationOptions` | Configure authentication provider and allowed scopes for this route. | 設定此路由的驗證提供者與允許的範圍 (Scopes)。 |
| 14 | Authorization Options | `Tip_AuthorizationOptions` | Define required claims and allowed values for route access control. | 定義存取此路由所需的 Claims 與允許值。 |
| 15 | Rate Limit Options | `Tip_RateLimitOptions` | Control request rate limiting: period, limit count, and client whitelist. | 控制請求速率限制：週期、限制次數及客戶端白名單。 |
| 16 | Load Balancer Options | `Tip_LoadBalancerOptions` | Load balancing strategy across downstream hosts: RoundRobin, LeastConnection, or NoLoadBalancer. | 下游主機的負載均衡策略：輪詢 (RoundRobin)、最少連線 (LeastConnection) 或不啟用。 |
| 17 | QoS Options | `Tip_QoSOptions` | Quality of Service settings: circuit breaker thresholds, timeout exceptions, and duration of break. | 服務品質設定：斷路器閾值、逾時例外數量及中斷持續時間。 |
| 18 | Cache Options | `Tip_CacheOptions` | Response caching: TTL (seconds), cache region, and custom header for cache key. | 回應快取設定：存活時間 (TTL)、快取區域及自訂快取鍵標頭。 |
| 19 | Security Options | `Tip_SecurityOptions` | IP allow/block lists for route-level security filtering. | 路由層級的 IP 允許/封鎖清單安全篩選。 |
| 20 | HTTP Handler Options | `Tip_HttpHandlerOptions` | HTTP handler behavior: allow auto-redirect, use cookie container, use tracing, use proxy. | HTTP 處理器行為：自動重導向、使用 Cookie 容器、啟用追蹤、使用代理。 |

---

## 實作 Checklist

### UI 變更
- [ ] RouteEdit.razor: Upstream Path Template 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: HTTP Method Selector 包裹 `MudTooltip`
- [ ] RouteEdit.razor: Upstream Host 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: Downstream Path Template 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: Scheme 選擇器包裹 `MudTooltip`
- [ ] RouteEdit.razor: HTTP Version 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: Downstream Host Editor 包裹 `MudTooltip`
- [ ] RouteEdit.razor: Key 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: Priority 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: Timeout 欄位包裹 `MudTooltip`
- [ ] RouteEdit.razor: Case Sensitive checkbox 包裹 `MudTooltip`
- [ ] RouteEdit.razor: Accept Any Cert checkbox 包裹 `MudTooltip`
- [ ] RouteEdit.razor: 8 個 Advanced JSON expansion panels 各自包裹 `MudTooltip`
- [ ] DownstreamHostEditor.razor: Host / Port 欄位包裹 `MudTooltip`（可選，已被外層涵蓋）
- [ ] HttpMethodSelector.razor: 整個選擇器已被外層 `MudTooltip` 涵蓋，不需額外處理

### i18n 資源
- [ ] SharedResource.resx: 新增 20 組 `Tip_*` 英文資源鍵
- [ ] SharedResource.zh-TW.resx: 新增 20 組 `Tip_*` 繁中資源鍵

### 驗證
- [ ] `dotnet build` — 0 errors, 0 warnings
- [ ] 確認 Tooltip 在表單上正確顯示，不影響欄位互動

---

## MudTooltip 使用範式

```razor
@* 包裹 MudTextField *@
<MudTooltip Text="@L["Tip_UpstreamPath"]" Arrow="true" Placement="Placement.Top">
    <MudTextField T="string"
                  Label="@L["RouteEdit_PathTemplate"]"
                  @bind-Value="_route.UpstreamPathTemplate"
                  ... />
</MudTooltip>

@* 包裹 MudCheckBox *@
<MudTooltip Text="@L["Tip_CaseSensitive"]" Arrow="true" Placement="Placement.Top">
    <MudCheckBox T="bool?"
                 Label="@L["RouteEdit_CaseSensitive"]"
                 @bind-Value="_route.RouteIsCaseSensitive" />
</MudTooltip>

@* 包裹 Advanced Section *@
<MudTooltip Text="@L["Tip_AuthenticationOptions"]" Arrow="true" Placement="Placement.Top">
    <MudExpansionPanel Text="@current.Name">
        ...
    </MudExpansionPanel>
</MudTooltip>
```

**統一風格**：`Arrow="true"` + `Placement="Placement.Top"`

---

## 預計檔案路徑

### Web Layer（修改）
- `src/Web/Components/Pages/Routes/RouteEdit.razor` — 主要修改
- `src/Web/Resources/SharedResource.resx` — 新增 Tip_* keys
- `src/Web/Resources/SharedResource.zh-TW.resx` — 新增 Tip_* keys

### 不需新增檔案
此功能只修改現有檔案，不需新增任何 .cs 或 .razor 檔案。

---

## 相依項目

### 依賴的元件
- `MudTooltip`：MudBlazor 內建，已在專案中引用
- `IStringLocalizer<SharedResource>`：已在所有 Page/Component 注入

### 依賴的外部服務
- 無

---

## 備註

- `MudTooltip` 不影響子元件的事件綁定與資料流
- Advanced JSON Section 的 `MudExpansionPanel` 本身在迴圈中渲染，tooltip key 需對應 `_advancedSections` 陣列順序
- 為 Advanced Section 添加 tooltip，需在 `AdvancedSection` record 中增加一個 `TooltipKey` 欄位
