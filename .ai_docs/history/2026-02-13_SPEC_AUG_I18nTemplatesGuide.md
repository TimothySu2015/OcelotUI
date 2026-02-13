# SPEC: AUG - 多國語系 + 路由範例 + 使用教學

**功能名稱**：AUG_I18nTemplatesGuide
**狀態**：✅ 已完成
**建立日期**：2026-02-13
**基於**：SPEC_OcelotRouteEditor.md

---

## 1. 功能概述

在現有 OcelotUI 路由編輯器上疊加三項功能：

| # | 功能 | 說明 |
|---|------|------|
| A | 多國語系 (i18n) | 繁體中文 / English 切換，AppBar 語言選擇器 |
| B | 路由範例模板 | RouteEdit 頁面提供預設範例，一鍵套用到表單 |
| C | 使用教學 | 新頁面 `/guide`，說明 Ocelot 與本工具的使用方式 |

**影響範圍**：僅 Web Layer（純 UI 變更），不影響 Domain / Application / Infrastructure。

---

## 2. 功能 A：多國語系 (i18n)

### 2.1 技術方案

使用 ASP.NET Core 內建 `Microsoft.Extensions.Localization`：
- `IStringLocalizer<SharedResource>` 注入各元件
- `.resx` 資源檔存放翻譯字串
- 語言切換透過 `CultureInfo.CurrentUICulture` 控制
- 使用者偏好存於 Cookie（`CookieRequestCultureProvider`）
- 支援語系：`en`（預設）、`zh-TW`

### 2.2 資源檔結構

**檔案位置**：`src/Web/Resources/`

```
Resources/
├── SharedResource.cs              ← 空類別，做為 IStringLocalizer<T> 的類型錨點
├── SharedResource.en.resx         ← 英文（預設）
└── SharedResource.zh-TW.resx      ← 繁體中文
```

### 2.3 翻譯 Key 清單

#### 共用
| Key | en | zh-TW |
|-----|----|-------|
| AppTitle | OcelotUI - Configuration Editor | OcelotUI - 配置編輯器 |
| Nav_Routes | Routes | 路由管理 |
| Nav_Guide | Guide | 使用教學 |
| Btn_Save | Save | 儲存 |
| Btn_Cancel | Cancel | 取消 |
| Btn_Delete | Delete | 刪除 |
| Btn_Add | Add | 新增 |
| Saving | Saving... | 儲存中... |
| Confirm | Confirm | 確認 |

#### RouteList 頁
| Key | en | zh-TW |
|-----|----|-------|
| RouteList_Title | Ocelot Routes | Ocelot 路由列表 |
| RouteList_Search | Search by Upstream Path... | 依上游路徑搜尋... |
| RouteList_AddRoute | Add Route | 新增路由 |
| RouteList_Col_Upstream | Upstream | 上游 |
| RouteList_Col_Downstream | Downstream | 下游 |
| RouteList_Col_Key | Key | 鍵值 |
| RouteList_Col_Actions | Actions | 操作 |
| RouteList_DeleteConfirm | Are you sure you want to delete route | 確定要刪除此路由嗎 |
| RouteList_DeleteSuccess | Route deleted successfully. | 路由刪除成功。 |
| RouteList_LoadFailed | Failed to load routes. | 載入路由失敗。 |

#### RouteEdit 頁
| Key | en | zh-TW |
|-----|----|-------|
| RouteEdit_NewTitle | New Route | 新增路由 |
| RouteEdit_EditTitle | Edit Route #{0} | 編輯路由 #{0} |
| RouteEdit_Upstream | Upstream | 上游設定 |
| RouteEdit_Downstream | Downstream | 下游設定 |
| RouteEdit_PathTemplate | Path Template | 路徑模板 |
| RouteEdit_HttpMethods | HTTP Methods | HTTP 方法 |
| RouteEdit_Host | Host (optional) | 主機（選填） |
| RouteEdit_Scheme | Scheme | 協議 |
| RouteEdit_HttpVersion | HTTP Version (optional) | HTTP 版本（選填） |
| RouteEdit_HostAndPorts | Downstream Host & Ports | 下游主機與端口 |
| RouteEdit_AddHost | Add Host | 新增主機 |
| RouteEdit_Options | Options | 選項 |
| RouteEdit_Key | Key | 鍵值 |
| RouteEdit_Priority | Priority | 優先級 |
| RouteEdit_Timeout | Timeout (sec) | 逾時（秒） |
| RouteEdit_CaseSensitive | Case Sensitive | 區分大小寫 |
| RouteEdit_AcceptAnyCert | Accept Any Certificate | 接受任何憑證 |
| RouteEdit_Advanced | Advanced (JSON) | 進階設定 (JSON) |
| RouteEdit_Required | {0} is required | {0} 為必填欄位 |
| RouteEdit_SaveSuccess | Route saved successfully. | 路由儲存成功。 |
| RouteEdit_NotFound | Route not found. | 找不到路由。 |
| RouteEdit_InvalidJson | Invalid JSON format. | JSON 格式無效。 |
| RouteEdit_ApplyTemplate | Apply Template | 套用範例 |
| RouteEdit_TemplateApplied | Template "{0}" applied. | 已套用範例「{0}」。 |

### 2.4 語言切換元件

**檔案**：`src/Web/Components/Shared/CultureSelector.razor`

**位置**：嵌入 MainLayout 的 AppBar 右側

**行為**：
- 下拉選單顯示 `English` / `繁體中文`
- 選擇後設定 Cookie `.AspNetCore.Culture`
- 透過 `NavigationManager.NavigateTo(..., forceLoad: true)` 重新載入頁面以套用新語系

### 2.5 受影響的現有檔案

| 檔案 | 變更 |
|------|------|
| `Program.cs` | 加入 `AddLocalization()`、`AddRequestLocalizationOptions()` |
| `_Imports.razor` | 加入 `@using Microsoft.Extensions.Localization` |
| `MainLayout.razor` | AppBar 加入 `<CultureSelector />`，所有文字改用 `@L["Key"]` |
| `NavMenu.razor` | 所有文字改用 `@L["Key"]` |
| `RouteList.razor` | 所有硬編碼文字改用 `@L["Key"]` |
| `RouteEdit.razor` | 所有硬編碼文字改用 `@L["Key"]` |
| `HttpMethodSelector.razor` | Label 改用 `@L["Key"]` |
| `DownstreamHostEditor.razor` | Label 改用 `@L["Key"]` |

---

## 3. 功能 B：路由範例模板

### 3.1 設計概念

在 RouteEdit 頁面的工具列加入「Apply Template」按鈕，點擊後彈出對話框列出預定義的路由範例，選擇後將範例值填入表單欄位。

### 3.2 範例清單

| # | 名稱 | 說明 |
|---|------|------|
| 1 | Basic REST API Proxy | 基本 REST API 代理，GET/POST/PUT/DELETE → 下游服務 |
| 2 | Single GET Endpoint | 單一 GET 端點代理 |
| 3 | WebSocket Proxy | WebSocket 連線代理 |
| 4 | Path with Catch-All | 使用 `{everything}` 萬用路徑代理整個服務 |
| 5 | Rate Limited API | 含限流設定的 API 代理 |
| 6 | Authenticated API | 含認證設定的 API 代理 |
| 7 | Load Balanced | 多目標主機負載均衡 |

### 3.3 範例模板定義

**檔案**：`src/Web/Services/RouteTemplateProvider.cs`

```
public class RouteTemplate
{
    public string Name { get; set; }           // 範例名稱（翻譯 Key）
    public string Description { get; set; }    // 說明（翻譯 Key）
    public OcelotRoute Route { get; set; }     // 預填的路由物件
}
```

**Static Provider**，不需要 DI，直接提供 `IReadOnlyList<RouteTemplate>`。

### 3.4 範例模板內容（精選 3 個展示）

#### Basic REST API Proxy
```json
{
  "UpstreamPathTemplate": "/api/users/{everything}",
  "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"],
  "DownstreamPathTemplate": "/api/users/{everything}",
  "DownstreamScheme": "https",
  "DownstreamHostAndPorts": [{ "Host": "user-service", "Port": 443 }]
}
```

#### WebSocket Proxy
```json
{
  "UpstreamPathTemplate": "/ws/{everything}",
  "UpstreamHttpMethod": ["GET"],
  "DownstreamPathTemplate": "/ws/{everything}",
  "DownstreamScheme": "wss",
  "DownstreamHostAndPorts": [{ "Host": "ws-service", "Port": 443 }]
}
```

#### Path with Catch-All
```json
{
  "UpstreamPathTemplate": "/service-a/{everything}",
  "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE", "PATCH"],
  "DownstreamPathTemplate": "/{everything}",
  "DownstreamScheme": "https",
  "DownstreamHostAndPorts": [{ "Host": "service-a.internal", "Port": 443 }]
}
```

### 3.5 UI 互動

**觸發位置**：RouteEdit 頁面工具列（Save / Cancel 旁邊）

```
[Apply Template ▼]  [Cancel]  [Save]
```

**流程**：
1. 點擊「Apply Template」→ 彈出 `MudMenu` 或 `MudDialog`
2. 顯示範例清單（名稱 + 簡短說明）
3. 點擊某範例 → 表單欄位被覆蓋為範例值
4. Snackbar 提示 `Template "Basic REST API Proxy" applied.`
5. 使用者可自行修改後儲存

**注意**：如果表單已有資料，套用範例前顯示確認對話框「Current data will be overwritten. Continue?」

### 3.6 新增檔案

| 檔案 | 說明 |
|------|------|
| `src/Web/Services/RouteTemplateProvider.cs` | 範例模板定義 |

### 3.7 受影響的現有檔案

| 檔案 | 變更 |
|------|------|
| `RouteEdit.razor` | 工具列加入 Apply Template 按鈕 + 套用邏輯 |

---

## 4. 功能 C：使用教學

### 4.1 頁面路由

`/guide`

### 4.2 內容結構

使用 MudBlazor 的 `MudTimeline` 或 `MudStepper` 呈現步驟式教學：

| 步驟 | 標題 | 內容 |
|------|------|------|
| 1 | What is Ocelot? | Ocelot 是 .NET 的 API Gateway 框架。ocelot.json 用來定義路由規則。 |
| 2 | Configure File Path | 在 `appsettings.json` 設定 `OcelotConfig.FilePath` 指向你的 ocelot.json 位置。 |
| 3 | Route Basics | 每條路由定義 Upstream（客戶端請求路徑）和 Downstream（目標服務路徑）的映射。 |
| 4 | Managing Routes | 使用路由列表頁面檢視所有路由。點擊 ✏️ 編輯、🗑️ 刪除。 |
| 5 | Creating a Route | 點擊「Add Route」，填寫 Upstream 和 Downstream 設定，或使用「Apply Template」快速套用範例。 |
| 6 | Advanced Settings | 展開 Options 和 Advanced 區塊，可以設定認證、限流、負載均衡等進階功能。 |
| 7 | Saving Changes | 點擊 Save 後，變更會直接寫回 ocelot.json 檔案。 |

所有文字透過 `IStringLocalizer` 提供中英文版本。

### 4.3 新增檔案

| 檔案 | 說明 |
|------|------|
| `src/Web/Components/Pages/Guide/GuidePage.razor` | 使用教學頁面 |

### 4.4 受影響的現有檔案

| 檔案 | 變更 |
|------|------|
| `NavMenu.razor` | 加入 Guide 導航連結 |

---

## 5. 完整檔案清單

### 新增檔案

| 檔案 | 說明 |
|------|------|
| `src/Web/Resources/SharedResource.cs` | IStringLocalizer 類型錨點 |
| `src/Web/Resources/SharedResource.en.resx` | 英文資源檔 |
| `src/Web/Resources/SharedResource.zh-TW.resx` | 繁體中文資源檔 |
| `src/Web/Components/Shared/CultureSelector.razor` | 語言切換元件 |
| `src/Web/Services/RouteTemplateProvider.cs` | 路由範例模板 |
| `src/Web/Components/Pages/Guide/GuidePage.razor` | 使用教學頁面 |

### 修改檔案

| 檔案 | 變更摘要 |
|------|----------|
| `Program.cs` | 加入 Localization 服務與中介軟體 |
| `_Imports.razor` | 加入 `@using Microsoft.Extensions.Localization` |
| `MainLayout.razor` | AppBar 加入 CultureSelector，文字 i18n |
| `NavMenu.razor` | 加入 Guide 連結，文字 i18n |
| `RouteList.razor` | 所有硬編碼文字 → `@L["Key"]` |
| `RouteEdit.razor` | 加入 Apply Template，所有硬編碼文字 → `@L["Key"]` |
| `HttpMethodSelector.razor` | Label i18n |
| `DownstreamHostEditor.razor` | Label i18n |

---

## 6. 影響分析

### Breaking Changes
- **無**。純 UI 層變更，不修改 Domain / Application / Infrastructure。

### 風險評估
| 項目 | 風險 | 說明 |
|------|------|------|
| 語系切換 | 低 | 使用 ASP.NET Core 標準機制，成熟穩定 |
| 範例模板 | 低 | 純靜態資料，無副作用 |
| 教學頁面 | 低 | 獨立新頁面，不影響現有功能 |

### 不影響的層
- Domain：無變更
- Application：無變更
- Infrastructure：無變更

---

## 7. 驗收標準

### 功能 A：多國語系
- [ ] AppBar 右側顯示語言切換下拉
- [ ] 選擇「繁體中文」後，所有 UI 文字切換為中文
- [ ] 選擇「English」後，所有 UI 文字切換為英文
- [ ] 重新整理頁面後語言偏好仍保留（Cookie）
- [ ] 預設語言為 English

### 功能 B：路由範例模板
- [ ] RouteEdit 工具列顯示「Apply Template」按鈕
- [ ] 點擊後列出 7 個預定義範例
- [ ] 選擇範例後表單欄位正確填入
- [ ] 已有資料時套用前顯示確認對話框
- [ ] 範例名稱與說明支援 i18n

### 功能 C：使用教學
- [ ] `/guide` 頁面可訪問
- [ ] NavMenu 顯示 Guide 連結
- [ ] 教學內容包含 7 個步驟
- [ ] 教學文字支援 i18n（中英切換）
