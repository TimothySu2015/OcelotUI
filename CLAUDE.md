# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 溝通語言

一律使用**繁體中文**回覆與撰寫註解、commit message、PR 說明等文字內容。

## 專案概述

OcelotUI 是一個 Blazor Server 應用程式，用於視覺化編輯 Ocelot API Gateway 的 `ocelot.json` 配置文件。無資料庫，純檔案 I/O 搭配原子寫入。內建版本歷史功能，每次儲存自動建立快照。

## 建置與執行

```bash
dotnet build                                               # 建置所有專案
dotnet run --project src/Web                               # 啟動開發伺服器 (https://localhost:53993)
dotnet run --project src/Web --urls http://localhost:5001   # 自訂埠號執行
```

本專案目前無測試專案。

## 技術棧

| 項目 | 版本/選擇 |
|------|----------|
| .NET | 10, C# 13 |
| UI | Blazor Server (InteractiveServer), MudBlazor 8.x |
| 架構 | Clean Architecture + CQRS (MediatR) |
| JSON | System.Text.Json |
| Nullable | 啟用 |
| 警告轉錯誤 | 啟用 (TreatWarningsAsErrors) |

## 架構

```
Web (Blazor UI) → Application (CQRS) → Domain (Entities)
       ↓                ↑
  Infrastructure (檔案 I/O)
```

**Domain** — 純 POCO，對應 ocelot.json 結構。每個實體皆有 `[JsonExtensionData]` 以確保向前相容。無外部相依。

**Application** — MediatR 處理器，遵循命名慣例：`Get*Query`、`Add*Command`、`Update*Command`、`Delete*Command`。所有操作回傳 `Result<T>` 處理錯誤。驗證邏輯在 Command Handler 中。

**Infrastructure** — `OcelotFileConfigurationRepository` 負責讀寫 ocelot.json。使用原子寫入（暫存檔 + `File.Move`）。JSON 選項：`WriteIndented = true`、`DefaultIgnoreCondition = WhenWritingNull`、`UnsafeRelaxedJsonEscaping`。每次 `SaveAsync` 自動透過 `ISnapshotRepository` 建立快照。`FileSnapshotRepository` 將快照存放於 `.ocelot-history/` 目錄，上限 50 筆，超過自動刪除最舊的。

**Web** — Blazor Server 頁面直接將 Domain 實體綁定到 MudBlazor 表單元件。子屬性編輯器（Cache、QoS、Auth 等）使用 `Value`/`ValueChanged` 雙向綁定模式，搭配「空值即 null」語意。

## 關鍵模式

### CQRS 慣例
- Commands/Queries 使用 `record` 定義，Handler 為獨立 class（primary constructor 注入相依）。
- 驗證邏輯直接寫在 Handler 中（無 FluentValidation），失敗回傳 `Result.Failure<T>("message")`。
- `Result<T>` 定義在 `Application/Common/Result.cs`，提供 `IsSuccess`/`IsFailure`/`Value`/`Error` 屬性。

```csharp
// 在 Blazor 頁面中的標準用法
var result = await Mediator.Send(new GetRouteByIndexQuery(index));
if (result.IsFailure) { Snackbar.Add(string.Format(L["Error"], result.Error), Severity.Error); return; }
var dto = result.Value;

// 儲存
var result = IsNew
    ? await Mediator.Send(new AddRouteCommand(dto))
    : await Mediator.Send(new UpdateRouteCommand(index, dto));
```

### 子屬性編輯器模式
每個編輯器接收 `Value`/`ValueChanged` 參數，使用 `Update()` 輔助方法：若物件為 null 則建立，套用變更後，若所有欄位為空則發出 null：
```csharp
[Parameter] public CacheOptions? Value { get; set; }
[Parameter] public EventCallback<CacheOptions?> ValueChanged { get; set; }

private void Update(Action<CacheOptions> apply) {
    var v = Value ?? new CacheOptions();
    apply(v);
    ValueChanged.InvokeAsync(IsEmpty(v) ? null : v);
}
```

### 快照/版本歷史
每次儲存 ocelot.json 自動建立快照，也可手動建立。歷史頁面 (`/history`) 支援：
- 列表瀏覽（時間、描述、路由數、檔案大小）
- 單筆預覽、還原、刪除
- 勾選兩筆進行 side-by-side JSON 差異比對（BlazorMonaco `StandaloneDiffEditor`）
- 清除全部歷史

快照以檔案形式存於 `.ocelot-history/` 目錄（與 ocelot.json 同層），`snapshots.json` 為索引檔。ID 格式：`yyyy-MM-ddTHHmmss_random2bytes`。還原前會自動建立備份快照。

### JSON 預覽面板狀態
`JsonPreviewState`（Scoped 服務）管理側欄 JSON 預覽面板。提供 `Toggle()`、`Lock()/Unlock()`、`SetFocus(PreviewSection, int?)` 方法及 `OnChange`/`OnFocusChange` 事件。變更配置後呼叫 `PreviewState.NotifyChanged()` 刷新面板。

### 路由範本
`RouteTemplateProvider` 定義 7 種預設路由範本（BasicRestApi、WebSocket、RateLimited 等），用於新增路由時快速套用。

### 國際化 (i18n)
- 資源檔：`src/Web/Resources/SharedResource.resx`（英文）與 `SharedResource.zh-TW.resx`（繁中）
- 錨點類別：`SharedResource.cs`（空 class，作為 `IStringLocalizer<SharedResource>` 的型別標記）
- 使用方式：`@inject IStringLocalizer<SharedResource> L`，然後 `L["KeyName"]`
- 命名規則：`{Section}_{Property}` 為標籤，`{Section}_{Property}_Desc` 為提示說明
- 語系切換：透過 `/api/set-culture?culture=zh-TW&redirectUri=/` 端點，以 Cookie 儲存偏好

## MudBlazor 慣例

- **MudBlazor 8 要求 HTML 屬性必須小寫**（例如 `title` 而非 `Title`）。MUD0002 分析器會強制執行，違規會變成建置錯誤。
- 在 `MudIconButton` 上加提示，使用 `MudTooltip` 包裹或小寫 `title` 屬性。

### 頁面路由慣例
頁面使用多個 `@page` 指令區分新增與編輯模式：
```razor
@page "/routes/new"            ← IsNew = true（Index 為 null）
@page "/routes/edit/{Index:int}" ← 編輯模式
```
同模式套用於 `/aggregates`、`/dynamic-routes`。特殊頁面：`/global-config`、`/history`、`/guide`。

## 重要檔案位置

| 用途 | 路徑 |
|------|------|
| 方案檔 | `OcelotUI.slnx` |
| 共用建置設定 | `Directory.Build.props` |
| DI 註冊 | `src/Web/Program.cs`、`src/Infrastructure/DependencyInjection.cs` |
| Repository 實作 | `src/Infrastructure/Persistence/OcelotFileConfigurationRepository.cs` |
| 快照 Repository | `src/Infrastructure/Persistence/FileSnapshotRepository.cs` |
| 快照實體 | `src/Domain/Entities/ConfigSnapshot.cs` |
| 快照 CQRS 處理器 | `src/Application/Snapshots/` |
| 歷史頁面 | `src/Web/Components/Pages/History/` |
| 快照存放目錄 | `.ocelot-history/`（與 ocelot.json 同層，不進版控） |
| 全域 Razor imports | `src/Web/Components/_Imports.razor` |
| 共用元件 | `src/Web/Components/Shared/` |
| 領域實體 | `src/Domain/Entities/` |
| CQRS 處理器 | `src/Application/Routes/`、`src/Application/Aggregates/`、`src/Application/Snapshots/` 等 |
| Result 型別 | `src/Application/Common/Result.cs` |
| JSON 預覽面板狀態 | `src/Web/Services/JsonPreviewState.cs` |
| 路由範本 | `src/Web/Services/RouteTemplateProvider.cs` |
| 配置檔路徑 | 透過 `appsettings.json` → `OcelotConfig:FilePath` 設定（預設：`ocelot.json`） |

## 功能規格索引

| 功能 | SPEC | 狀態 |
|------|------|------|
| 路由編輯器 | `.ai_docs/history/2026-02-13_SPEC_OcelotRouteEditor.md` | ✅ 已完成 |
| i18n + 範例 + 教學 | `.ai_docs/history/2026-02-13_SPEC_AUG_I18nTemplatesGuide.md` | ✅ 已完成 |
| 強型別子屬性編輯器 | `.ai_docs/features/SPEC_AUG_TypedSubPropertyEditors.md` | ✅ 已完成 |
| 版本歷史（快照） | — | ✅ 已完成 |
