**繁體中文** | [简体中文](README.md) | [English](README-EN.md)

<div align="center">

<img src="PCLCY/Images/icon.ico" alt="PCL CY Edition Logo" width="80" height="80">

# PCL CY Edition

開源、免費、社群驅動的 Minecraft 啟動器

</div>

PCL CY Edition 是基於 [PCL 社群版（PCL CE）](https://github.com/PCL-Community/PCL-CE) 二次開發的開源 Minecraft 啟動器，而 PCL CE 又源自[龍騰貓躍](https://github.com/Meloong-Git/PCL)的 Plain Craft Launcher 2。PCL CY Edition 擁有獨立的品牌、資料目錄與更新線路，可與原版 PCL、PCL CE 同時安裝、互不干擾。

**原始碼、Releases 與問題回饋：<https://github.com/chenfengyimei/PCL3>**

PCL CY Edition 是完全開放的公開版本：不限制主題色系，不按國家或地區鎖定下載、補全或整合包安裝，也不隱藏任何內建功能。

## ✨ 功能

- 遊戲版本管理與全自動安裝（正式版、快照版、舊版）
- Forge、NeoForge、Fabric、Quilt、OptiFine 等模組載入器
- 模組、整合包、資源包、光影與世界的搜尋、下載和管理
- 微軟正版、離線、Authlib-Injector 登入與多帳號切換
- Java 自動偵測、下載與記憶體參數管理
- 智慧崩潰分析與日誌定位
- 主題、背景、自訂首頁（XAML）與啟動提示
- 百寶箱：垃圾清理、今日人品、自訂下載、面板與頭像、伺服器查詢、成就產生等
- 基於 EasyTier 的多人連線
- 多資料夾、版本隔離、匯出與分享

## 🔔 啟動說明

PCL CY Edition 會在**每個版本首次啟動**時展示一次版本說明，內容包括：

- 本啟動器基於 PCL CE 二次開發，與龍騰貓躍、PCL-Community 無從屬關係；
- PCL CY Edition 的原始碼依原授權條款公開，歡迎查閱與貢獻；
- 品牌與資料目錄獨立，不會自動更新為 PCL CE 官方版本。

該說明只會在版本變更後的第一次啟動出現，之後不會反覆打擾。

## 💻 執行環境

| 作業系統 | 支援情況 | 環境需求 |
|---|---|---|
| Windows 10 1809 (17763) 或更新 | ✅ 完整支援 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 – Windows 10 1809 | ⚠️ 理論可用，酌情支援 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 或更舊版本 | ❌ 不支援 | / |
| macOS / Linux | ⚠️ 僅交叉編譯開發 | [.NET 10 SDK](https://get.dot.net/10) |

## 🛠️ 從原始碼建置

```powershell
dotnet build "PCLCY.slnx" -c Release
```

建置產物為 `PCLCY/bin/Release/net10.0-windows/win-x64/PCLCY.exe`。

## 🔒 授權與開源

PCL CY Edition 是基於 PCL 開源程式碼的衍生作品，繼續遵循上游授權條款：

- `PCLCY/` 使用 [自訂授權條款](PCLCY/LICENCE)
- `其餘所有目錄` 使用 [Apache License 2.0](LICENSE)

依 PCL 授權條款要求，修改版需要公開其原始碼。PCL CY Edition 的完整原始碼託管於 <https://github.com/chenfengyimei/PCL3>，啟動器內 `設定 → 關於 → 原始碼` 指向同一儲存庫。

## 🙏 致謝

感謝龍騰貓躍創作 PCL / PCL2，感謝 PCL-Community 維護 PCL CE，以及所有為啟動器、鏡像源與社群內容做出貢獻的開發者。

- 上游 PCL：<https://github.com/Meloong-Git/PCL>
- 上游 PCL CE：<https://github.com/PCL-Community/PCL-CE>
- 問題回饋：<https://github.com/chenfengyimei/PCL3/issues>，請不要向原版或 PCL CE 儲存庫回饋 PCL CY Edition 的問題。
