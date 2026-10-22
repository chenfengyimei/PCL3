**English** | [简体中文](README.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="PCLCY/Images/icon.ico" alt="PCL CY Edition Logo" width="80" height="80">

# PCL CY Edition

An open-source, free, community-driven Minecraft launcher

[![.NET](https://img.shields.io/badge/.NET-10-512BD4)](https://get.dot.net/10) [![Platform](https://img.shields.io/badge/Platform-Windows-blue)](https://github.com/chenfengyimei/PCLCY/releases) [![License](https://img.shields.io/badge/License-Apache--2.0%20%2F%20PCL-green)](LICENSE)

</div>

## 📖 Introduction

PCL CY Edition is an open-source Minecraft launcher for Windows. It is derived from [PCL Community Edition (PCL CE)](https://github.com/PCL-Community/PCL-CE), which in turn derives from [LTCat](https://github.com/Meloong-Git/PCL)'s Plain Craft Launcher 2. It ships with its own branding, data directory and update channel, and can be installed alongside the original PCL and PCL CE without conflicts.

**Source code, releases and issue tracker: <https://github.com/chenfengyimei/PCLCY>**

PCL CY Edition is a fully open-source, free public build with no feature restrictions: every feature is available to every user in every region, with no donations, activation or unlocks required.

## ✨ Features

### Versions & Loaders
- Game version management and fully automatic installation (releases, snapshots, legacy Beta/Alpha)
- One-click install of Forge, NeoForge, Fabric, Quilt, OptiFine and other mod loaders
- Multiple game folders, instance isolation, export and sharing

### Resources
- Search, download and manage mods, modpacks, resource packs, shaders, datapacks and worlds
- CurseForge and Modrinth sources with MCIM mirror acceleration
- Mod conflict detection, dependency completion, disable management and local naming rules

### Accounts & Login
- Microsoft accounts, offline profiles, Authlib-Injector servers and multi-account switching
- Free choice of auth type when creating a profile (including a "skip Microsoft verification" shortcut to offline profiles)

### Launching & Optimization
- Automatic Java detection, download and memory management
- Toolbox "memory optimizer": one click to trim idle memory of the launcher and game processes
- Smart crash analysis with automatic cause detection and fix suggestions
- Live game log viewer and export

### Customization
- Custom themes: hue, saturation, lightness and hue-gradient sliders with live preview
- Built-in preset themes plus an HMCL tribute theme, with light/dark modes
- Custom XAML homepages: 20 built-in presets (trivia, echo cave, Minecraft news, skin picks, OpenBMCLAPI dashboard, magazine, bangumi and more), network loading and full customization
- Echo cave: a random player quote on every launch
- Theme backgrounds, background music, launch hints and April Fools' easter eggs

### Multiplayer & Tools
- Multiplayer lobbies powered by EasyTier (join by room code, no port forwarding)
- Server status query and ping
- Toolbox utilities: junk cleaning, daily luck, custom downloads, skin & avatar fetcher, achievement generator and more

### Updates & Maintenance
- GitHub Releases based auto-update: check, download and apply updates from inside the launcher
- A version notice shown once on the first launch of each version
- Full logging system (daily files, one-click export for feedback)

## 🔔 Startup notice

PCL CY Edition shows a version notice **once on the first launch of each version**. It explains that:

- PCL CY Edition is built on PCL CE and is not affiliated with LTCat or PCL-Community;
- PCL CY Edition's source code is published under the original licenses and contributions are welcome;
- Branding and data directories are independent, and updates point to this repository.

The notice appears only after a version change and never repeats on every launch.

## 🚀 Download & Install

1. Download the latest `PCLCY_*.exe` from [Releases](https://github.com/chenfengyimei/PCLCY/releases);
2. Put it in any writable folder (not a system or downloader folder) and run it directly;
3. The launcher creates a `PCL` folder beside the exe for local data, and `%AppData%\PCLCY` for global data.

No installation and no administrator privileges required; users of the original PCL or PCL CE can run both side by side without data conflicts.

> CurseForge downloads, Microsoft login and similar features rely on API keys injected at build time; official Release builds include them. See [Build from source](#️-build-from-source) when building yourself.

## 💻 Requirements

| OS | Support | Requirements |
|---|---|---|
| Windows 10 1809 (17763) or later | ✅ Fully supported | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 – Windows 10 1809 | ⚠️ Should work, best-effort support | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 or older | ❌ Not supported | / |
| macOS / Linux | ⚠️ Cross-compilation only | [.NET 10 SDK](https://get.dot.net/10) |

## 🛠️ Build from source

```powershell
# Build
dotnet build "PCLCY.slnx" -c Release

# Publish a single-file exe (x64)
dotnet publish "PCLCY/PCLCY.csproj" -p:Configuration=Release -p:Platform=x64 -o ./artifact --no-self-contained
```

The output is `PCLCY/bin/Release/net10.0-windows/win-x64/PCLCY.exe`.

Some features rely on build-time secrets (CurseForge API key, Microsoft login client ID, etc.) injected via environment variables:

```powershell
$env:PCL_WRITE_SECRET = '1'
$env:PCL_CURSEFORGE_API_KEY = '<your key>'
dotnet publish ...
```

Note: secrets are baked by a source generator; for repeated local builds, run `dotnet build-server shutdown` and clear `PCL.Core\obj` first, otherwise the shared compiler process keeps a stale environment snapshot.

## 📂 Repository layout

| Directory | Description |
|---|---|
| `PCLCY/` | Launcher project (WPF UI and business logic) |
| `PCL.Core/` | Core library (config, localization, networking, themes, launch protocol) |
| `PCL.Core.SourceGenerators/` | Source generators (secret injection and other compile-time codegen) |
| `PCL.Core.Test/` | Core library unit tests |
| `.github/` | CI workflows and issue templates |

## 🔒 License and open source

PCL CY Edition is a derivative work of PCL's open-source code and keeps the upstream licenses:

- `PCLCY/` uses the [custom license](PCLCY/LICENCE)
- All other directories use the [Apache License 2.0](LICENSE)

The PCL license requires modified editions to publish their source code. PCL CY Edition's full source is hosted at <https://github.com/chenfengyimei/PCLCY>; the in-app `About → Software Info → Source Code` button points to the same repository. Third-party dependencies and their licenses are listed in the launcher's About page.

## 🤝 Contributing & Feedback

- Bugs and feature requests: [Issues](https://github.com/chenfengyimei/PCLCY/issues)
- Discussions: [Discussions](https://github.com/chenfengyimei/PCLCY/discussions)
- Read [CONTRIBUTING.md](CONTRIBUTING.md) to get started with development

Please report PCL CY Edition issues to this repository, not to the original PCL or PCL CE repositories.

## 🙏 Credits

Thanks to LTCat for creating PCL / PCL2, to PCL-Community for maintaining PCL CE, and to everyone contributing launchers, mirrors (BMCLAPI, MCIM and more) and community content.

- Upstream PCL: <https://github.com/Meloong-Git/PCL>
- Upstream PCL CE: <https://github.com/PCL-Community/PCL-CE>
