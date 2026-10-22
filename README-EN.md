**English** | [简体中文](README.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="PCLCY/Images/icon.ico" alt="PCL CY Edition Logo" width="80" height="80">

# PCL CY Edition

An open-source, free, community-driven Minecraft launcher

</div>

PCL CY Edition is an open-source Minecraft launcher built on top of [PCL Community Edition (PCL CE)](https://github.com/PCL-Community/PCL-CE), which in turn derives from [LTCat](https://github.com/Meloong-Git/PCL)'s Plain Craft Launcher 2. PCL CY Edition ships with its own branding, data directory and update channel, so it can be installed alongside the original PCL and PCL CE without conflicts.

**Source code, releases and issue tracker: <https://github.com/chenfengyimei/PCL3>**

PCL CY Edition is a fully open public build: no locked theme palettes, no regional restrictions on downloads, repairs or modpack installation, and no hidden built-in features.

## ✨ Features

- Game version management and fully automatic installation (releases, snapshots, legacy versions)
- Forge, NeoForge, Fabric, Quilt and OptiFine support
- Search, download and manage mods, modpacks, resource packs, shaders and worlds
- Microsoft, offline and Authlib-Injector accounts with multi-account switching
- Java detection/download and memory management
- Smart crash analysis and log inspection
- Themes, backgrounds, custom XAML homepages and launch hints
- Toolbox: junk cleaning, daily luck, custom downloads, skins and avatars, server query, achievement generator and more
- Multiplayer powered by EasyTier
- Multiple game folders, version isolation, export and sharing

## 🔔 Startup notice

PCL CY Edition shows a version notice **once on the first launch of each version**. It explains that:

- PCL CY Edition is built on PCL CE and is not affiliated with LTCat or PCL-Community;
- PCL CY Edition's source code is published under the original licenses and contributions are welcome;
- Branding and data directories are independent, and PCL CY Edition never auto-updates itself into the official PCL CE.

The notice appears only after a version change and never repeats on every launch.

## 💻 Requirements

| OS | Support | Requirements |
|---|---|---|
| Windows 10 1809 (17763) or later | ✅ Fully supported | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 – Windows 10 1809 | ⚠️ Should work, best-effort support | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 or older | ❌ Not supported | / |
| macOS / Linux | ⚠️ Cross-compilation only | [.NET 10 SDK](https://get.dot.net/10) |

## 🛠️ Build from source

```powershell
dotnet build "PCLCY.slnx" -c Release
```

The output is `PCLCY/bin/Release/net10.0-windows/win-x64/PCLCY.exe`.

## 🔒 License and open source

PCL CY Edition is a derivative work of PCL's open-source code and keeps the upstream licenses:

- `PCLCY/` uses the [custom license](PCLCY/LICENCE)
- `All other directories` use the [Apache License 2.0](LICENSE)

The PCL license requires modified editions to publish their source code. PCL CY Edition's full source is hosted at <https://github.com/chenfengyimei/PCL3>; the in-app `Settings → About → Source Code` button points to the same repository.

## 🙏 Credits

Thanks to LTCat for creating PCL / PCL2, to PCL-Community for maintaining PCL CE, and to everyone who contributes launchers, mirrors and community content.

- Upstream PCL: <https://github.com/Meloong-Git/PCL>
- Upstream PCL CE: <https://github.com/PCL-Community/PCL-CE>
- Please report PCL CY Edition issues at <https://github.com/chenfengyimei/PCL3/issues>, not to the original PCL or PCL CE repositories.
