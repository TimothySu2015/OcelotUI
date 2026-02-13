# OcelotUI - Ocelot Configuration Editor

## 專案概述
ASP.NET Core Blazor Server 系統，用於視覺化編輯 Ocelot API Gateway 的 `ocelot.json` 配置文件。

## 技術棧
| 項目 | 版本/選擇 |
|------|----------|
| .NET | 10 |
| C# | 13 |
| UI | Blazor Server (InteractiveServer) |
| 架構 | Clean Architecture + CQRS |
| CQRS 中介 | MediatR |
| JSON | System.Text.Json |
| UI 元件庫 | MudBlazor |
| 資料庫 | 無（純檔案 I/O） |
| Nullable | 啟用 |
| 警告轉錯誤 | 啟用 |

## 目錄結構
```
/CLAUDE.md                          ← 專案索引（本檔案）
/.ai_docs/
  ├── ARCHITECTURE.md               ← 全域架構規範
  ├── features/
  │   └── SPEC_OcelotRouteEditor.md ← 路由編輯器 SPEC
  └── history/                      ← 已完成 SPEC 封存
/src/
  ├── Domain/                       ← 領域模型（Ocelot JSON 結構）
  │   ├── OcelotUI.Domain.csproj
  │   ├── Common/                   ← BaseEntity, IDomainEvent
  │   └── Entities/                 ← OcelotConfiguration, OcelotRoute...
  ├── Application/                  ← CQRS Commands & Queries
  │   ├── OcelotUI.Application.csproj
  │   ├── Common/                   ← Result<T>
  │   ├── Interfaces/               ← IOcelotConfigurationRepository
  │   └── Routes/
  │       ├── Queries/              ← GetAllRoutes, GetRouteByIndex
  │       └── Commands/             ← AddRoute, UpdateRoute, DeleteRoute
  ├── Infrastructure/               ← 檔案 I/O 實作
  │   ├── OcelotUI.Infrastructure.csproj
  │   └── Persistence/             ← OcelotFileConfigurationRepository
  └── Web/                          ← Blazor Server 前端
      ├── OcelotUI.Web.csproj
      ├── Program.cs
      └── Components/
          └── Pages/
              ├── Routes/           ← RouteList, RouteEdit
              └── GlobalConfig/     ← GlobalConfigEdit

## 功能索引
| 功能 | SPEC | 狀態 |
|------|------|------|
| 路由編輯器 | `.ai_docs/history/2026-02-13_SPEC_OcelotRouteEditor.md` | ✅ 已完成 |
| i18n + 範例 + 教學 | `.ai_docs/history/2026-02-13_SPEC_AUG_I18nTemplatesGuide.md` | ✅ 已完成 |
| 強型別子屬性編輯器 | `.ai_docs/features/SPEC_AUG_TypedSubPropertyEditors.md` | ✅ 已完成 |
