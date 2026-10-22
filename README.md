**简体中文** | [English](README-EN.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="PCLCY/Images/icon.ico" alt="PCL CY Edition Logo" width="80" height="80">

# PCL CY Edition

开源、免费、社区驱动的 Minecraft 启动器

[![.NET](https://img.shields.io/badge/.NET-10-512BD4)](https://get.dot.net/10) [![Platform](https://img.shields.io/badge/平台-Windows-blue)](https://github.com/chenfengyimei/PCLCY/releases) [![License](https://img.shields.io/badge/许可证-Apache--2.0%20%2F%20PCL-green)](LICENSE)

</div>

## 📖 项目简介

PCL CY Edition 是一款面向 Windows 平台的开源 Minecraft 启动器，由 [PCL 社区版（PCL CE）](https://github.com/PCL-Community/PCL-CE) 二次开发而来，而 PCL CE 源自[龙腾猫跃](https://github.com/Meloong-Git/PCL)的 Plain Craft Launcher 2。它拥有独立的品牌、数据目录与更新线路，可与原版 PCL、PCL CE 同时安装、互不干扰。

**源码、Releases 与问题反馈：<https://github.com/chenfengyimei/PCLCY>**

PCL CY Edition 是完全开源、免费、没有任何功能限制的公开版本：所有功能对全部用户与全部地区一视同仁，不需要赞助、激活或任何解锁操作。

## ✨ 功能

### 版本与加载器
- 游戏版本管理与全自动安装（正式版、快照版、旧版 Beta/Alpha）
- Forge、NeoForge、Fabric、Quilt、OptiFine 等模组加载器一键安装
- 多游戏文件夹、版本隔离、实例导入导出与分享

### 资源生态
- 模组、整合包、资源包、光影、数据包与世界的搜索、下载和管理
- CurseForge 与 Modrinth 双资源源，支持 MCIM 镜像加速
- 模组冲突检测、依赖补全、禁用管理与本地命名规则

### 账号与登录
- 微软正版登录、离线档案、Authlib-Injector 第三方验证与多账号切换
- 新建档案时可自由选择验证方式（含"跳过正版验证"直达离线档案）

### 启动与优化
- Java 自动检测、下载与内存参数管理
- 百宝箱"内存优化"：一键释放启动器与游戏进程的闲置内存
- 智能崩溃分析、自动识别崩溃原因并给出修复建议
- 实时游戏日志查看与导出

### 个性化
- 自定义主题：色相、饱和度、明度、色相渐变四参数自由调配，随调随生效
- 内置多套预设主题与 HMCL 致敬主题，支持亮/暗双模式
- 自定义主页（XAML）：20 个内置预设（你知道吗、回声洞、Minecraft 新闻、皮肤推荐、OpenBMCLAPI 仪表盘、杂志主页、番剧主页等），也支持网络加载与完全自定义
- 回声洞：每次启动随机展示一条玩家语录
- 主题背景、背景音乐、启动提示、愚人节彩蛋等细节定制

### 联机与工具
- 基于 EasyTier 的多人联机大厅（房间号直连，免端口映射）
- 服务器状态查询与 ping
- 垃圾清理、今日人品、自定义下载、皮肤与头像获取、成就生成器等百宝箱工具

### 更新与维护
- 基于 GitHub Releases 的自动更新：启动器内一键检查、下载并完成更新
- 每个版本首次启动展示一次版本说明
- 完善的日志系统（按天分文件，支持一键导出打包反馈）

## 🔔 启动说明

PCL CY Edition 会在**每个版本首次启动**时展示一次版本说明，内容包括：

- 本启动器基于 PCL CE 二次开发，与龙腾猫跃、PCL-Community 无从属关系；
- PCL CY Edition 的源代码按原许可证公开，欢迎查阅与贡献；
- 品牌与数据目录独立，更新线路指向本仓库。

该说明只会在版本变化后的第一次启动出现，之后不会反复打扰。

## 🚀 下载与安装

1. 前往 [Releases](https://github.com/chenfengyimei/PCLCY/releases) 下载最新版 `PCLCY_*.exe`；
2. 将其放入任意有写入权限的文件夹后直接运行（推荐非系统盘、非下载工具目录）；
3. 启动器会自动在所在目录创建 `PCL` 文件夹存放配置与缓存，在 `%AppData%\PCLCY` 存放全局数据。

无需安装、无需管理员权限；已安装原版 PCL 或 PCL CE 的用户可并存使用，互不影响数据。

> 下载 CurseForge 资源、正版登录等功能依赖构建时注入的 API 密钥，官方 Releases 包已内置；从源码自行构建时这些功能按 [构建指南](#️-从源码构建) 配置。

## 💻 运行环境

| 操作系统 | 支持情况 | 环境要求 |
|---|---|---|
| Windows 10 1809 (17763) 或更高 | ✅ 完整支持 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 – Windows 10 1809 | ⚠️ 理论可用，酌情支持 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 或更低版本 | ❌ 不支持 | / |
| macOS / Linux | ⚠️ 仅交叉编译开发 | [.NET 10 SDK](https://get.dot.net/10) |

## 🛠️ 从源码构建

```powershell
# 构建
dotnet build "PCLCY.slnx" -c Release

# 发布单文件 exe（x64）
dotnet publish "PCLCY/PCLCY.csproj" -p:Configuration=Release -p:Platform=x64 -o ./artifact --no-self-contained
```

构建产物为 `PCLCY/bin/Release/net10.0-windows/win-x64/PCLCY.exe`。

部分功能依赖构建时注入的密钥（CurseForge API Key、微软登录 Client ID 等），通过环境变量注入：

```powershell
$env:PCL_WRITE_SECRET = '1'
$env:PCL_CURSEFORGE_API_KEY = '<你的 Key>'
dotnet publish ...
```

注意：密钥经源生成器写入程序集，本地连续构建需先 `dotnet build-server shutdown` 并清理 `PCL.Core\obj`，否则编译服务进程的环境快照会保留旧值。

## 📂 目录结构

| 目录 | 说明 |
|---|---|
| `PCLCY/` | 启动器主工程（WPF UI 与业务逻辑） |
| `PCL.Core/` | 核心库（配置、本地化、网络、主题、启动协议等） |
| `PCL.Core.SourceGenerators/` | 源生成器（密钥注入等编译期代码生成） |
| `PCL.Core.Test/` | 核心库单元测试 |
| `.github/` | CI 工作流与 Issue 模板 |

## 🔒 许可证与开源

PCL CY Edition 是基于 PCL 开源代码的衍生作品，继续遵循上游许可证：

- `PCLCY/` 使用 [自定义许可证](PCLCY/LICENCE)
- 其余所有目录使用 [Apache License 2.0](LICENSE)

根据 PCL 许可证要求，修改版需要公开其源代码。PCL CY Edition 的完整源码托管于 <https://github.com/chenfengyimei/PCLCY>，启动器内 `关于 → 软件信息 → 源代码` 指向同一仓库。第三方依赖及其许可证详见启动器内 `关于 → 软件信息` 的第三方许可声明。

## 🤝 贡献与反馈

- 问题反馈与功能建议：[Issues](https://github.com/chenfengyimei/PCLCY/issues)
- 讨论交流：[Discussions](https://github.com/chenfengyimei/PCLCY/discussions)
- 参与开发请阅读 [CONTRIBUTING.md](CONTRIBUTING.md)

请将 PCL CY Edition 的问题反馈到本仓库，而不是原版 PCL 或 PCL CE 的仓库。

## 🙏 致谢

感谢龙腾猫跃创作 PCL / PCL2，感谢 PCL-Community 维护 PCL CE，以及所有为启动器、镜像源（BMCLAPI、MCIM 等）和社区内容做出贡献的开发者。

- 上游 PCL：<https://github.com/Meloong-Git/PCL>
- 上游 PCL CE：<https://github.com/PCL-Community/PCL-CE>
