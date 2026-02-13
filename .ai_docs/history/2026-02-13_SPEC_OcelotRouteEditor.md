# SPEC: Ocelot Route Editor

**功能名稱**：OcelotRouteEditor
**狀態**：✅ 已完成
**建立日期**：2026-02-13

---

## 1. 功能概述

提供 Blazor Server 互動式介面，讓使用者可以：
1. **列出**所有 Ocelot Routes
2. **新增**路由（含 Upstream/Downstream 設定）
3. **編輯**既有路由
4. **刪除**路由（含確認對話框）
5. **儲存**修改回 `ocelot.json` 檔案

---

## 2. Domain Models

### 2.1 OcelotConfiguration（根聚合）

**檔案**：`src/Domain/Entities/OcelotConfiguration.cs`

```
OcelotConfiguration
├── Routes: List<OcelotRoute>
├── DynamicRoutes: List<DynamicRoute>?          // 本期不編輯，保留原值
├── Aggregates: List<Aggregate>?                // 本期不編輯，保留原值
└── GlobalConfiguration: OcelotGlobalConfiguration?
```

### 2.2 OcelotRoute

**檔案**：`src/Domain/Entities/OcelotRoute.cs`

核心欄位（UI 完整編輯）：

| 欄位 | 型別 | 說明 |
|------|------|------|
| UpstreamPathTemplate | string | 上游路徑模板，如 `/api/users/{id}` |
| UpstreamHttpMethod | List\<string\> | 允許的 HTTP 方法：Get, Post, Put, Delete, Patch, Options |
| UpstreamHost | string? | 上游主機過濾 |
| DownstreamPathTemplate | string | 下游路徑模板 |
| DownstreamScheme | string | `http` 或 `https` |
| DownstreamHostAndPorts | List\<DownstreamHostAndPort\> | 下游目標主機列表 |
| DownstreamHttpMethod | string? | 下游 HTTP 方法覆寫 |
| DownstreamHttpVersion | string? | HTTP 版本（1.0, 1.1, 2.0） |

擴展欄位（UI 以 JSON 原始編輯器呈現）：

| 欄位 | 型別 | 說明 |
|------|------|------|
| Key | string? | 路由鍵值，用於 Aggregates 引用 |
| Priority | int? | 路由優先級（數值越低優先級越低） |
| RouteIsCaseSensitive | bool | 路徑是否區分大小寫 |
| AuthenticationOptions | object? | 認證選項 |
| RateLimitOptions | object? | 限流選項 |
| LoadBalancerOptions | object? | 負載均衡選項 |
| QoSOptions | object? | 服務品質選項 |
| CacheOptions | object? | 快取選項 |
| HttpHandlerOptions | object? | HTTP 處理器選項 |
| SecurityOptions | object? | 安全選項（IP 白/黑名單） |
| Timeout | int? | 超時秒數 |
| DangerousAcceptAnyServerCertificateValidator | bool | 是否接受任意 SSL 憑證 |
| Metadata | Dictionary\<string, string\>? | 自訂中繼資料 |
| AddHeadersToRequest | Dictionary\<string, string\>? | 新增標頭到請求 |
| UpstreamHeaderTemplates | Dictionary\<string, string\>? | 上游標頭模板 |
| DownstreamHeaderTransform | Dictionary\<string, string\>? | 下游標頭轉換 |
| UpstreamHeaderTransform | Dictionary\<string, string\>? | 上游標頭轉換 |
| DelegatingHandlers | List\<string\>? | 委派處理器 |

### 2.3 DownstreamHostAndPort

**檔案**：`src/Domain/Entities/DownstreamHostAndPort.cs`

```
DownstreamHostAndPort
├── Host: string       // 如 "localhost", "192.168.1.100"
└── Port: int          // 如 5001, 443
```

### 2.4 OcelotGlobalConfiguration

**檔案**：`src/Domain/Entities/OcelotGlobalConfiguration.cs`

本期僅做**顯示**，不提供編輯 UI（未來擴展）。
使用 `JsonExtensionData` 保留所有原始欄位，確保讀寫不遺失資料。

---

## 3. Application Layer (CQRS)

### 3.1 Repository Interface

**檔案**：`src/Application/Interfaces/IOcelotConfigurationRepository.cs`

```csharp
public interface IOcelotConfigurationRepository
{
    Task<OcelotConfiguration> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(OcelotConfiguration configuration, CancellationToken ct = default);
}
```

### 3.2 DTOs

**RouteListItemDto**（列表用，精簡欄位）

**檔案**：`src/Application/Routes/Queries/GetAllRoutes/RouteListItemDto.cs`

| 欄位 | 型別 | 說明 |
|------|------|------|
| Index | int | 在 Routes 陣列中的索引 |
| UpstreamPathTemplate | string | 上游路徑 |
| UpstreamHttpMethod | List\<string\> | HTTP 方法 |
| DownstreamPathTemplate | string | 下游路徑 |
| DownstreamScheme | string | 協議 |
| DownstreamHosts | string | 格式化的主機列表，如 `localhost:5001, api.com:443` |
| Key | string? | 路由鍵值 |

**RouteDetailDto**（編輯用，完整欄位）

**檔案**：`src/Application/Routes/Queries/GetRouteByIndex/RouteDetailDto.cs`

包含 `OcelotRoute` 的所有欄位，一對一映射。

### 3.3 Queries

#### GetAllRoutesQuery

**檔案**：`src/Application/Routes/Queries/GetAllRoutes/`

| 項目 | 值 |
|------|---|
| Request | `GetAllRoutesQuery : IRequest<Result<List<RouteListItemDto>>>` |
| Handler | 讀取 Repository → 映射為 `RouteListItemDto` 列表 |
| 回傳 | `Result<List<RouteListItemDto>>` |

#### GetRouteByIndexQuery

**檔案**：`src/Application/Routes/Queries/GetRouteByIndex/`

| 項目 | 值 |
|------|---|
| Request | `GetRouteByIndexQuery(int Index) : IRequest<Result<RouteDetailDto>>` |
| Handler | 讀取 Repository → 驗證 Index 邊界 → 映射為 `RouteDetailDto` |
| 回傳 | `Result<RouteDetailDto>` |
| 錯誤 | Index 超出範圍回傳 `Result.Failure` |

### 3.4 Commands

#### AddRouteCommand

**檔案**：`src/Application/Routes/Commands/AddRoute/`

| 項目 | 值 |
|------|---|
| Request | `AddRouteCommand(RouteDetailDto Route) : IRequest<Result<int>>` |
| Handler | 讀取 Repository → 新增 Route 到列表尾部 → SaveAsync → 回傳新 Index |
| 驗證 | UpstreamPathTemplate 不可為空、DownstreamHostAndPorts 至少一筆 |
| 回傳 | `Result<int>`（新路由的索引） |

#### UpdateRouteCommand

**檔案**：`src/Application/Routes/Commands/UpdateRoute/`

| 項目 | 值 |
|------|---|
| Request | `UpdateRouteCommand(int Index, RouteDetailDto Route) : IRequest<Result>` |
| Handler | 讀取 Repository → 驗證 Index → 替換 Routes[Index] → SaveAsync |
| 驗證 | Index 有效、UpstreamPathTemplate 不可為空 |
| 回傳 | `Result` |

#### DeleteRouteCommand

**檔案**：`src/Application/Routes/Commands/DeleteRoute/`

| 項目 | 值 |
|------|---|
| Request | `DeleteRouteCommand(int Index) : IRequest<Result>` |
| Handler | 讀取 Repository → 驗證 Index → RemoveAt(Index) → SaveAsync |
| 回傳 | `Result` |

---

## 4. Infrastructure Layer

### 4.1 OcelotFileConfigurationRepository

**檔案**：`src/Infrastructure/Persistence/OcelotFileConfigurationRepository.cs`

**配置**：
```json
// appsettings.json
{
  "OcelotConfig": {
    "FilePath": "C:\\path\\to\\ocelot.json"
  }
}
```

**Options 類別**：`src/Infrastructure/Persistence/OcelotConfigOptions.cs`
```csharp
public class OcelotConfigOptions
{
    public const string SectionName = "OcelotConfig";
    public string FilePath { get; set; } = "ocelot.json";
}
```

**實作要點**：
- `LoadAsync`：`File.ReadAllTextAsync` → `JsonSerializer.Deserialize<OcelotConfiguration>`
- `SaveAsync`：
  1. 序列化為 JSON 字串（WriteIndented = true）
  2. 寫入暫存檔 `{FilePath}.tmp`
  3. `File.Move(tmpPath, filePath, overwrite: true)` 原子替換
- 若檔案不存在，`LoadAsync` 回傳空的 `OcelotConfiguration`（Routes = []）
- `JsonSerializerOptions` 統一配置：
  - `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`
  - `WriteIndented = true`
  - `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull`

### 4.2 DI 註冊

**檔案**：`src/Infrastructure/DependencyInjection.cs`

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OcelotConfigOptions>(
            configuration.GetSection(OcelotConfigOptions.SectionName));
        services.AddScoped<IOcelotConfigurationRepository,
            OcelotFileConfigurationRepository>();
        return services;
    }
}
```

---

## 5. Web Layer (Blazor Server)

### 5.1 頁面規劃

#### 5.1.1 Route List (`/routes`)

**檔案**：`src/Web/Components/Pages/Routes/RouteList.razor`

**UI 元件**：MudDataGrid

**欄位**：
| 列 | 寬度 | 內容 |
|----|------|------|
| # | 60px | 索引值 |
| Upstream | auto | `[Methods] PathTemplate` |
| Downstream | auto | `Scheme://Host:Port + PathTemplate` |
| Key | 120px | 路由 Key（可選） |
| Actions | 120px | Edit / Delete 圖示按鈕 |

**行為**：
- 頁面載入時發送 `GetAllRoutesQuery`
- 「Add Route」按鈕 → 導航至 `/routes/new`
- Edit 圖示 → 導航至 `/routes/edit/{index}`
- Delete 圖示 → 彈出 `MudDialog` 確認 → 發送 `DeleteRouteCommand` → 重新載入列表
- 支援搜尋過濾（依 UpstreamPathTemplate 篩選）

#### 5.1.2 Route Edit (`/routes/edit/{index}` & `/routes/new`)

**檔案**：`src/Web/Components/Pages/Routes/RouteEdit.razor`

**UI 佈局**：

```
┌─────────────────────────────────────────────────┐
│  Route Editor          [Save] [Cancel]          │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌─ Upstream ─────────────────────────────────┐ │
│  │ Path Template:  [/api/users/{id}        ]  │ │
│  │ HTTP Methods:   [Get] [Post] [Put] [Del]   │ │
│  │ Host (optional):[                       ]  │ │
│  └────────────────────────────────────────────┘ │
│                                                 │
│  ┌─ Downstream ───────────────────────────────┐ │
│  │ Path Template:  [/api/users/{id}        ]  │ │
│  │ Scheme:         [https ▼]                  │ │
│  │ HTTP Version:   [       ] (optional)       │ │
│  │                                            │ │
│  │ Host & Ports:                              │ │
│  │  ┌──────────────┬──────┬────┐              │ │
│  │  │ Host         │ Port │    │              │ │
│  │  ├──────────────┼──────┼────┤              │ │
│  │  │ localhost    │ 5001 │ 🗑 │              │ │
│  │  │ api.com      │ 443  │ 🗑 │              │ │
│  │  └──────────────┴──────┴────┘              │ │
│  │  [+ Add Host]                              │ │
│  └────────────────────────────────────────────┘ │
│                                                 │
│  ┌─ Options ──────────────────────────────────┐ │
│  │ Key:            [                       ]  │ │
│  │ Priority:       [1    ]                    │ │
│  │ Case Sensitive: [ ] No                     │ │
│  │ Accept Any Cert:[ ] No                     │ │
│  │ Timeout (sec):  [      ]                   │ │
│  └────────────────────────────────────────────┘ │
│                                                 │
│  ┌─ Advanced (JSON) ─────────────────────────┐  │
│  │ ▶ Authentication Options                  │  │
│  │ ▶ Rate Limit Options                      │  │
│  │ ▶ Load Balancer Options                   │  │
│  │ ▶ QoS Options                             │  │
│  │ ▶ Cache Options                           │  │
│  │ ▶ Security Options                        │  │
│  │ ▶ HTTP Handler Options                    │  │
│  │ ▶ Header Transforms                       │  │
│  │ ▶ Metadata                                │  │
│  └────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

**行為**：
- `/routes/new`：空表單，Submit → `AddRouteCommand`
- `/routes/edit/{index}`：載入時 `GetRouteByIndexQuery` → 填充表單，Submit → `UpdateRouteCommand`
- Advanced 區塊：每個選項展開後為 JSON 原始編輯器（MudTextField Multiline），允許進階使用者直接編輯 JSON
- 表單驗證：UpstreamPathTemplate 必填、至少一個 DownstreamHostAndPort
- Save 成功後導航回 `/routes`
- Cancel 導航回 `/routes`（不儲存）

### 5.2 共用元件

#### DownstreamHostEditor

**檔案**：`src/Web/Components/Shared/DownstreamHostEditor.razor`

**功能**：編輯 `List<DownstreamHostAndPort>` 的可重用元件
- 顯示現有 Host:Port 列表
- 新增一筆 Host:Port
- 刪除單筆
- 雙向繫結回父元件

#### HttpMethodSelector

**檔案**：`src/Web/Components/Shared/HttpMethodSelector.razor`

**功能**：多選 HTTP Method 的 Chip 選擇器
- 預設選項：GET, POST, PUT, DELETE, PATCH, OPTIONS, HEAD
- 使用 `MudChipSet` 多選模式
- 雙向繫結 `List<string>`

### 5.3 Navigation

**檔案**：`src/Web/Components/Layout/NavMenu.razor`

```
📋 Routes        → /routes
⚙️ Global Config → /global-config (Phase 2)
```

### 5.4 DI 註冊

**檔案**：`src/Web/Program.cs`

```csharp
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetAllRoutesQuery).Assembly));
builder.Services.AddInfrastructure(builder.Configuration);
```

---

## 6. 完整檔案清單

### Domain (src/Domain/)
| 檔案 | 說明 |
|------|------|
| `OcelotUI.Domain.csproj` | 專案檔（無外部依賴） |
| `Common/BaseEntity.cs` | ✅ 已建立 |
| `Common/IDomainEvent.cs` | ✅ 已建立 |
| `Entities/OcelotConfiguration.cs` | 根聚合 |
| `Entities/OcelotRoute.cs` | 路由實體 |
| `Entities/DownstreamHostAndPort.cs` | 下游主機 |
| `Entities/OcelotGlobalConfiguration.cs` | 全域配置 |

### Application (src/Application/)
| 檔案 | 說明 |
|------|------|
| `OcelotUI.Application.csproj` | 專案檔（依賴 MediatR, Domain） |
| `Common/Result.cs` | ✅ 已建立 |
| `Interfaces/IOcelotConfigurationRepository.cs` | Repository 介面 |
| `Routes/Queries/GetAllRoutes/GetAllRoutesQuery.cs` | Query 定義 |
| `Routes/Queries/GetAllRoutes/GetAllRoutesQueryHandler.cs` | Query Handler |
| `Routes/Queries/GetAllRoutes/RouteListItemDto.cs` | 列表 DTO |
| `Routes/Queries/GetRouteByIndex/GetRouteByIndexQuery.cs` | Query 定義 |
| `Routes/Queries/GetRouteByIndex/GetRouteByIndexQueryHandler.cs` | Query Handler |
| `Routes/Queries/GetRouteByIndex/RouteDetailDto.cs` | 詳細 DTO |
| `Routes/Commands/AddRoute/AddRouteCommand.cs` | Command 定義 |
| `Routes/Commands/AddRoute/AddRouteCommandHandler.cs` | Command Handler |
| `Routes/Commands/UpdateRoute/UpdateRouteCommand.cs` | Command 定義 |
| `Routes/Commands/UpdateRoute/UpdateRouteCommandHandler.cs` | Command Handler |
| `Routes/Commands/DeleteRoute/DeleteRouteCommand.cs` | Command 定義 |
| `Routes/Commands/DeleteRoute/DeleteRouteCommandHandler.cs` | Command Handler |

### Infrastructure (src/Infrastructure/)
| 檔案 | 說明 |
|------|------|
| `OcelotUI.Infrastructure.csproj` | 專案檔（依賴 Application, System.Text.Json） |
| `DependencyInjection.cs` | DI 註冊擴充方法 |
| `Persistence/OcelotConfigOptions.cs` | 檔案路徑 Options |
| `Persistence/OcelotFileConfigurationRepository.cs` | 檔案讀寫實作 |

### Web (src/Web/)
| 檔案 | 說明 |
|------|------|
| `OcelotUI.Web.csproj` | 專案檔（Blazor Server, MudBlazor, MediatR） |
| `Program.cs` | 應用程式進入點 & DI 配置 |
| `appsettings.json` | 含 OcelotConfig.FilePath 設定 |
| `Components/App.razor` | Blazor 根元件 |
| `Components/_Imports.razor` | 全域 using |
| `Components/Layout/MainLayout.razor` | MudBlazor 主佈局 |
| `Components/Layout/NavMenu.razor` | 導航選單 |
| `Components/Pages/Routes/RouteList.razor` | 路由列表頁 |
| `Components/Pages/Routes/RouteEdit.razor` | 路由編輯頁 |
| `Components/Shared/DownstreamHostEditor.razor` | 下游主機編輯元件 |
| `Components/Shared/HttpMethodSelector.razor` | HTTP 方法多選元件 |
| `wwwroot/` | 靜態資源 |

---

## 7. 驗收標準

- [ ] 頁面載入能正確解析並顯示既有 ocelot.json 的所有路由
- [ ] 可新增路由，新增後 JSON 檔案包含新路由
- [ ] 可編輯路由的 Upstream/Downstream 所有核心欄位，儲存後 JSON 正確更新
- [ ] 可刪除路由，刪除後 JSON 檔案不再包含該路由
- [ ] 寫回檔案時保留未編輯的欄位（如 GlobalConfiguration、DynamicRoutes）
- [ ] null 值欄位不寫入 JSON（保持檔案乾淨）
- [ ] JSON 格式化輸出（WriteIndented）
- [ ] 檔案路徑可透過 appsettings.json 配置
- [ ] 檔案不存在時顯示空列表，而非錯誤
