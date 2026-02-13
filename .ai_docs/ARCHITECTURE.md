# OcelotUI 全域架構規範

## Clean Architecture 四層定義

```
┌─────────────────────────────────────┐
│           Web (Blazor Server)       │  ← Presentation Layer
├─────────────────────────────────────┤
│         Application (CQRS)          │  ← Use Cases / Business Logic
├─────────────────────────────────────┤
│        Infrastructure (I/O)         │  ← File Persistence
├─────────────────────────────────────┤
│           Domain (Models)           │  ← Core Entities
└─────────────────────────────────────┘
```

### 依賴方向
- Web → Application → Domain
- Infrastructure → Application → Domain
- **Domain 不依賴任何外層**

---

## Layer 1: Domain

**職責**：定義 Ocelot 配置的領域模型，純 POCO，不含任何框架依賴。

**專案**：`OcelotUI.Domain`

**關鍵類別**：
- `OcelotConfiguration` — 根聚合，對應 ocelot.json 頂層結構
- `OcelotRoute` — 單一路由配置，包含 Upstream/Downstream 所有屬性
- `DownstreamHostAndPort` — 下游主機與端口
- `OcelotGlobalConfiguration` — 全域配置

**原則**：
- 所有屬性對應 Ocelot JSON Schema
- 使用 `System.Text.Json` 屬性標記（`[JsonPropertyName]`）確保序列化一致
- Nullable Reference Types 全面啟用

---

## Layer 2: Application

**職責**：實作 CQRS 的 Commands 與 Queries，透過 MediatR Pipeline 處理。

**專案**：`OcelotUI.Application`

**依賴**：MediatR

**結構**：
```
Application/
├── Common/
│   └── Result.cs                    ← 統一回傳型別
├── Interfaces/
│   └── IOcelotConfigurationRepository.cs  ← 持久層抽象
└── Routes/
    ├── Queries/
    │   ├── GetAllRoutes/
    │   │   ├── GetAllRoutesQuery.cs
    │   │   ├── GetAllRoutesQueryHandler.cs
    │   │   └── RouteListItemDto.cs
    │   └── GetRouteByIndex/
    │       ├── GetRouteByIndexQuery.cs
    │       ├── GetRouteByIndexQueryHandler.cs
    │       └── RouteDetailDto.cs
    └── Commands/
        ├── AddRoute/
        │   ├── AddRouteCommand.cs
        │   └── AddRouteCommandHandler.cs
        ├── UpdateRoute/
        │   ├── UpdateRouteCommand.cs
        │   └── UpdateRouteCommandHandler.cs
        └── DeleteRoute/
            ├── DeleteRouteCommand.cs
            └── DeleteRouteCommandHandler.cs
```

**CQRS 原則**：
- Query 不改變狀態，永遠回傳 `Result<T>`
- Command 改變狀態，回傳 `Result` 或 `Result<T>`
- 每個 Handler 職責單一，便於測試與替換

---

## Layer 3: Infrastructure

**職責**：實作 `IOcelotConfigurationRepository`，負責 ocelot.json 的讀寫。

**專案**：`OcelotUI.Infrastructure`

**關鍵實作**：
- `OcelotFileConfigurationRepository`
  - 透過 `appsettings.json` 配置 ocelot.json 檔案路徑
  - 讀取：`File.ReadAllTextAsync` → `JsonSerializer.Deserialize`
  - 寫入：序列化 → 寫入暫存檔 → `File.Move` 原子覆蓋（防止寫入中斷導致檔案損毀）
  - `JsonSerializerOptions { WriteIndented = true }` 保持可讀性

---

## Layer 4: Web (Presentation)

**職責**：Blazor Server 互動式 UI，透過 MediatR 發送 Query/Command。

**專案**：`OcelotUI.Web`

**UI 框架**：MudBlazor

**頁面結構**：
- `/routes` — 路由列表（MudDataGrid）
- `/routes/new` — 新增路由表單
- `/routes/edit/{index}` — 編輯路由表單
- `/global-config` — 全域配置編輯（未來擴展）

---

## 資料流

```
[使用者操作 Blazor UI]
        │
        ▼
[MediatR.Send(Query/Command)]
        │
        ▼
[Handler 呼叫 IOcelotConfigurationRepository]
        │
        ▼
[Repository 讀寫 ocelot.json 檔案]
```

---

## JSON 序列化策略

- 使用 `System.Text.Json`，不用 Newtonsoft
- `JsonSerializerOptions`:
  - `PropertyNamingPolicy = null`（預設 PascalCase，符合 Ocelot JSON 格式）
  - `WriteIndented = true`
  - `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull`（null 欄位不寫入 JSON，保持檔案乾淨）
- Domain Model 直接做為序列化目標（此專案無複雜行為，不需分離 Persistence Model）

## 併發控制

- 單用戶桌面工具，不實作樂觀併發
- Repository 每次操作重新讀取檔案，確保最新狀態
- 寫入使用暫存檔 + rename 策略，避免檔案損毀
