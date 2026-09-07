# Moe-TradeMarker

[中文说明](README.zh-CN.md)

[Forge page](https://sp-mod.com/mod/2736/moe-trademarker)

Moe-TradeMarker helps you tell trader-bought items apart at a glance. When you buy something from an NPC trader, the mod shows a small trader badge on the item in your client, so you can quickly see where it came from and avoid mixing trader stock with raid loot or other items. By default, it can also stop marked trader-bought items from being listed on the flea market, making trader resale rules easier to enforce in your SPT profile.

## What You Get

- Small trader markers on items bought from NPC traders.
- Trader names in item hover tooltips.
- Optional flea market protection for marked trader-bought items.
- Global and per-trader settings if you want to loosen or disable the rules.
- Client options for marker position and color, defaulting to the bottom-left position on new installs.
- Expanded multilingual display support for player-facing marker text.
- Auto language mode that follows the detected game language when possible.
- Optional BlackHawk QuickSell support disables `QuickSell (Flea)` for restricted items.

## Installation

Download the latest release archive and extract it into your SPT root directory. The archive contains:

`BepInEx/plugins/Moe-TradeMarker/`
`SPT_Runtime/user/mods/Moe-TradeMarker/`

If Moe-TradeMarker does not appear in the F12 `plugin / mod settings` menu, the installed client DLL is usually not the real `[BepInPlugin]` plugin DLL, or it was not placed under `BepInEx/plugins/Moe-TradeMarker/`.

## Configuration

Server options are in `SPT_Runtime/user/mods/Moe-TradeMarker/config.json` after installation.

Client display options can be changed from the BepInEx configuration menu:

- `ShowTraderMarker`: show or hide the trader marker.
- `MarkerPosition`: `LeftTop`, `RightTop`, `LeftBottom`, `RightBottom`; new installs default to `LeftBottom`.
- `MarkerColor`: marker icon color, `1.00 0.50 0.50 1.00` by default.
- `LanguageMode`: `Auto` follows the detected game language; manual values use SPT-supported language codes.

## Compatibility

Built for SPT 4.1.x. Tested against SPT 4.1.5 and EFT 0.16.9.5.40743.

Player-facing text is localized for SPT-supported languages including English, Chinese, French, German, Italian, Japanese, Korean, Polish, Portuguese, Russian, Spanish, and Turkish. English is used as the fallback.

## Build

Install the .NET 10 SDK alongside any existing SDKs. The server uses SPTushonka 4.1.5 packages; the client requires the actual SPT 4.1.5 / EFT 0.16.9.5.40743 assemblies, BepInEx core, spt-common and Newtonsoft.Json from your installation.

```powershell
dotnet build .\MoeTradeMarker.sln -c Release -p:SPTPath=C:\SPT
.\Package.ps1 -SPTPath C:\SPT
```

Building alone does not create a release ZIP. Package.ps1 rebuilds into an isolated directory, runs all tests, validates assembly and plugin versions, and then creates dist/Moe-TradeMarker-<version>.zip. A failed build or test leaves existing archives unchanged.

Language choices show their native names; saved enum values such as Chinese remain compatible. Configuration labels follow the selected language, and Auto follows the current game language.
