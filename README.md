# Moe-TradeMarker

[中文说明](README.zh-CN.md)

[Forge page](https://forge.sp-tarkov.com/mod/2736/moe-trademarker)

Moe-TradeMarker helps you tell trader-bought items apart at a glance. When you buy something from an NPC trader, the mod shows a small trader badge on the item in your client, so you can quickly see where it came from and avoid mixing trader stock with raid loot or other items. By default, it can also stop marked trader-bought items from being listed on the flea market, making trader resale rules easier to enforce in your SPT profile.

## What You Get

- Small trader markers on items bought from NPC traders.
- Trader names in item hover tooltips.
- Optional flea market protection for marked trader-bought items.
- Global and per-trader settings if you want to loosen or disable the rules.
- Client options for marker position and color.
- Expanded multilingual display support for player-facing marker text.
- Auto language mode that follows the detected game language when possible.

## Installation

Download the latest release archive and extract it into your SPT root directory. The archive contains:

`BepInEx/plugins/Moe-TradeMarker/`
`SPT/user/mods/Moe-TradeMarker/`

If Moe-TradeMarker does not appear in the F12 `plugin / mod settings` menu, the installed client DLL is usually not the real `[BepInPlugin]` plugin DLL, or it was not placed under `BepInEx/plugins/Moe-TradeMarker/`.

## Configuration

Server options are in `SPT/user/mods/Moe-TradeMarker/config.json` after installation.

Client display options can be changed from the BepInEx configuration menu:

- `ShowTraderMarker`: show or hide the trader marker.
- `MarkerPosition`: `LeftTop`, `RightTop`, `LeftBottom`, `RightBottom`.
- `MarkerColor`: marker icon color, `1.00 0.50 0.50 1.00` by default.
- `LanguageMode`: `Auto` follows the detected game language; manual values use SPT-supported language codes.

## Compatibility

Built for SPT 4.0+. Tested against SPTarkov 4.0.13.

Player-facing text is localized for SPT-supported languages including English, Chinese, French, German, Italian, Japanese, Korean, Polish, Portuguese, Russian, Spanish, and Turkish. English is used as the fallback.

## Build

The server project depends on SPTarkov NuGet packages `4.0.13`.

The client project builds a real BepInEx plugin with NuGet reference assemblies by default. You can also set `SPTPath` to your SPT installation directory so the build prefers local SPT client assemblies:

```powershell
dotnet build .\MoeTradeMarker.sln -p:SPTPath=C:\SPT
```
