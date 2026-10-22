**简体中文** | [English](README-EN.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="PCLCY/Images/icon.ico" alt="PCL CY Edition Logo" width="80" height="80">

# PCL CY Edition

开源、免费、社区驱动的 Minecraft 启动器

</div>

PCL CY Edition 是基于 [PCL 社区版（PCL CE）](https://github.com/PCL-Community/PCL-CE) 二次开发的开源 Minecraft 启动器，而 PCL CE 又源自[龙腾猫跃](https://github.com/Meloong-Git/PCL)的 Plain Craft Launcher 2。PCL CY Edition 拥有独立的品牌、数据目录与更新线路，可与原版 PCL、PCL CE 同时安装、互不干扰。

**源码、Releases 与问题反馈：<https://github.com/chenfengyimei/PCL3>**

PCL CY Edition 是完全开放的公开版本：不限制主题色系，不按国家或地区锁定下载、补全或整合包安装，也不隐藏任何内置功能。

## ✨ 功能

- 游戏版本管理与全自动安装（正式版、快照版、旧版）
- Forge、NeoForge、Fabric、Quilt、OptiFine 等模组加载器
- 模组、整合包、资源包、光影与世界的搜索、下载和管理
- 微软正版、离线、Authlib-Injector 登录与多账号切换
- Java 自动检测、下载与内存参数管理
- 智能崩溃分析与日志定位
- 主题、背景、自定义主页（XAML）与启动提示
- 百宝箱：垃圾清理、今日人品、自定义下载、皮肤与头像、服务器查询、成就生成等
- 基于 EasyTier 的多人联机
- 多文件夹、版本隔离、导出与分享

## 🔔 启动说明

PCL CY Edition 会在**每个版本首次启动**时展示一次版本说明，内容包括：

- 本启动器基于 PCL CE 二次开发，与龙腾猫跃、PCL-Community 无从属关系；
- PCL CY Edition 的源代码按原许可证公开，欢迎查阅与贡献；
- 品牌与数据目录独立，不会自动更新为 PCL CE 官方版本。

该说明只会在版本变化后的第一次启动出现，之后不会反复打扰。

## 💻 运行环境

| 操作系统 | 支持情况 | 环境要求 |
|---|---|---|
| Windows 10 1809 (17763) 或更高 | ✅ 完整支持 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 – Windows 10 1809 | ⚠️ 理论可用，酌情支持 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 或更低版本 | ❌ 不支持 | / |
| macOS / Linux | ⚠️ 仅交叉编译开发 | [.NET 10 SDK](https://get.dot.net/10) |

## 🛠️ 从源码构建

```powershell
dotnet build "PCLCY.slnx" -c Release
```

构建产物为 `PCLCY/bin/Release/net10.0-windows/win-x64/PCLCY.exe`（单文件、自包含依赖需按需添加发布参数）。

## 🔒 许可证与开源

PCL CY Edition 是基于 PCL 开源代码的衍生作品，继续遵循上游许可证：

- `PCLCY/` 使用 [自定义许可证](PCLCY/LICENCE)
- `其余所有目录` 使用 [Apache License 2.0](LICENSE)

根据 PCL 许可证要求，修改版需要公开其源代码。PCL CY Edition 的完整源码托管于 <https://github.com/chenfengyimei/PCL3>，启动器内 `设置 → 关于 → 源代码` 指向同一仓库。

## 🙏 致谢

感谢龙腾猫跃创作 PCL / PCL2，感谢 PCL-Community 维护 PCL CE，以及所有为启动器、镜像源和社区内容做出贡献的开发者。

- 上游 PCL：<https://github.com/Meloong-Git/PCL>
- 上游 PCL CE：<https://github.com/PCL-Community/PCL-CE>
- 问题反馈：<https://github.com/chenfengyimei/PCL3/issues>，请不要向原版或 PCL CE 仓库反馈 PCL CY Edition 的问题。
