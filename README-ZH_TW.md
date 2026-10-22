**繁體中文** | [简体中文](README.md) | [English](README-EN.md)

<div align="center">

<img src="PCLCY/Images/icon.ico" alt="PCL CY Edition Logo" width="80" height="80">

# PCL CY Edition

開源、免費、社群驅動的 Minecraft 啟動器

[![.NET](https://img.shields.io/badge/.NET-10-512BD4)](https://get.dot.net/10) [![Platform](https://img.shields.io/badge/平台-Windows-blue)](https://github.com/chenfengyimei/PCLCY/releases) [![License](https://img.shields.io/badge/授權-Apache--2.0%20%2F%20PCL-green)](LICENSE)

</div>

## 📖 專案簡介

PCL CY Edition 是一款面向 Windows 平台的開源 Minecraft 啟動器，由 [PCL 社群版（PCL CE）](https://github.com/PCL-Community/PCL-CE) 二次開發而來，而 PCL CE 源自[龍騰貓躍](https://github.com/Meloong-Git/PCL)的 Plain Craft Launcher 2。它擁有獨立的品牌、資料目錄與更新線路，可與原版 PCL、PCL CE 同時安裝、互不干擾。

**原始碼、Releases 與問題回饋：<https://github.com/chenfengyimei/PCLCY>**

PCL CY Edition 是完全開源、免費、沒有任何功能限制的公開版本：所有功能對全部使用者與全部地區一視同仁，不需要贊助、啟用或任何解鎖操作。

## ✨ 功能

### 版本與載入器
- 遊戲版本管理與全自動安裝（正式版、快照版、舊版 Beta/Alpha）
- Forge、NeoForge、Fabric、Quilt、OptiFine 等模組載入器一鍵安裝
- 多遊戲資料夾、版本隔離、實例匯入匯出與分享

### 資源生態
- 模組、整合包、資源包、光影、資料包與世界的搜尋、下載和管理
- CurseForge 與 Modrinth 雙資源源，支援 MCIM 鏡像加速
- 模組衝突檢測、依賴補全、停用管理與本地命名規則

### 帳號與登入
- 微軟正版登入、離線檔案、Authlib-Injector 第三方驗證與多帳號切換
- 新建檔案時可自由選擇驗證方式（含「跳過正版驗證」直達離線檔案）

### 啟動與最佳化
- Java 自動偵測、下載與記憶體參數管理
- 百寶箱「記憶體最佳化」：一鍵釋放啟動器與遊戲程序的閒置記憶體
- 智慧崩潰分析、自動識別崩潰原因並給出修復建議
- 即時遊戲日誌查看與匯出

### 個人化
- 自訂主題：色相、飽和度、明度、色相漸變四參數自由調配，隨調隨生效
- 內建多套預設主題與 HMCL 致敬主題，支援亮/暗雙模式
- 自訂首頁（XAML）：20 個內建預設（你知道吗、回聲洞、Minecraft 新聞、皮膚推薦、OpenBMCLAPI 儀表板、雜誌首頁、番劇首頁等），也支援網路載入與完全自訂
- 回聲洞：每次啟動隨機展示一條玩家語錄
- 主題背景、背景音樂、啟動提示、愚人節彩蛋等細節定制

### 聯機與工具
- 基於 EasyTier 的多人聯機大廳（房間號直連，免連接埠轉發）
- 伺服器狀態查詢與 ping
- 垃圾清理、今日人品、自訂下載、皮膚與頭像取得、成就產生器等百寶箱工具

### 更新與維護
- 基於 GitHub Releases 的自動更新：啟動器內一鍵檢查、下載並完成更新
- 每個版本首次啟動展示一次版本說明
- 完善的日誌系統（按天分檔案，支援一鍵匯出打包回饋）

## 🔔 啟動說明

PCL CY Edition 會在**每個版本首次啟動**時展示一次版本說明，內容包括：

- 本啟動器基於 PCL CE 二次開發，與龍騰貓躍、PCL-Community 無從屬關係；
- PCL CY Edition 的原始碼按原授權條款公開，歡迎查閱與貢獻；
- 品牌與資料目錄獨立，更新線路指向本儲存庫。

該說明只會在版本變化後的第一次啟動出現，之後不會反覆打擾。

## 🚀 下載與安裝

1. 前往 [Releases](https://github.com/chenfengyimei/PCLCY/releases) 下載最新版 `PCLCY_*.exe`；
2. 將其放入任意有寫入權限的資料夾後直接執行（推薦非系統碟、非下載工具目錄）；
3. 啟動器會自動在所在目錄建立 `PCL` 資料夾存放設定與快取，在 `%AppData%\PCLCY` 存放全域資料。

無需安裝、無需管理員權限；已安裝原版 PCL 或 PCL CE 的使用者可並存使用，互不影響資料。

> 下載 CurseForge 資源、正版登入等功能依賴建置時注入的 API 金鑰，官方 Releases 包已內建；從原始碼自行建置時這些功能按[建置指南](#️-從原始碼建置)設定。

## 💻 執行環境

| 作業系統 | 支援情況 | 環境需求 |
|---|---|---|
| Windows 10 1809 (17763) 或更高 | ✅ 完整支援 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 – Windows 10 1809 | ⚠️ 理論可用，酌情支援 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 或更低版本 | ❌ 不支援 | / |
| macOS / Linux | ⚠️ 僅交叉編譯開發 | [.NET 10 SDK](https://get.dot.net/10) |

## 🛠️ 從原始碼建置

```powershell
# 建置
dotnet build "PCLCY.slnx" -c Release

# 發布單檔 exe（x64）
dotnet publish "PCLCY/PCLCY.csproj" -p:Configuration=Release -p:Platform=x64 -o ./artifact --no-self-contained
```

建置產物為 `PCLCY/bin/Release/net10.0-windows/win-x64/PCLCY.exe`。

部分功能依賴建置時注入的金鑰（CurseForge API Key、微軟登入 Client ID 等），透過環境變數注入：

```powershell
$env:PCL_WRITE_SECRET = '1'
$env:PCL_CURSEFORGE_API_KEY = '<你的 Key>'
dotnet publish ...
```

注意：金鑰經原始碼產生器寫入程式集，本地連續建置需先 `dotnet build-server shutdown` 並清理 `PCL.Core\obj`，否則編譯服務程序的環境快照會保留舊值。

## 📂 目錄結構

| 目錄 | 說明 |
|---|---|
| `PCLCY/` | 啟動器主工程（WPF UI 與業務邏輯） |
| `PCL.Core/` | 核心庫（設定、本地化、網路、主題、啟動協定等） |
| `PCL.Core.SourceGenerators/` | 原始碼產生器（金鑰注入等編譯期程式碼產生） |
| `PCL.Core.Test/` | 核心庫單元測試 |
| `.github/` | CI 工作流程與 Issue 範本 |

## 🔒 授權與開源

PCL CY Edition 是基於 PCL 開源程式碼的衍生作品，繼續遵循上游授權條款：

- `PCLCY/` 使用[自訂授權條款](PCLCY/LICENCE)
- 其餘所有目錄使用 [Apache License 2.0](LICENSE)

依 PCL 授權條款要求，修改版需要公開其原始碼。PCL CY Edition 的完整原始碼託管於 <https://github.com/chenfengyimei/PCLCY>，啟動器內 `關於 → 軟體資訊 → 原始碼` 指向同一儲存庫。第三方相依套件及其授權詳見啟動器內 `關於 → 軟體資訊` 的第三方授權聲明。

## 🤝 貢獻與回饋

- 問題回饋與功能建議：[Issues](https://github.com/chenfengyimei/PCLCY/issues)
- 討論交流：[Discussions](https://github.com/chenfengyimei/PCLCY/discussions)
- 參與開發請閱讀 [CONTRIBUTING.md](CONTRIBUTING.md)

請將 PCL CY Edition 的問題回饋到本儲存庫，而不是原版 PCL 或 PCL CE 的儲存庫。

## 🙏 致謝

感謝龍騰貓躍創作 PCL / PCL2，感謝 PCL-Community 維護 PCL CE，以及所有為啟動器、鏡像源（BMCLAPI、MCIM 等）和社群內容做出貢獻的開發者。

- 上游 PCL：<https://github.com/Meloong-Git/PCL>
- 上游 PCL CE：<https://github.com/PCL-Community/PCL-CE>
