# Moe-TradeMarker

[中文说明](README.zh-CN.md)

[Forge page](https://forge.sp-tarkov.com/mod/2736/moe-trademarker)

Moe-TradeMarker is a combined SPT 4.0+ mod that marks items bought from NPC traders and can prevent marked items from being listed on the flea market.

## Features

- Adds `upd.tradeMarker.traderId` to items bought from NPC traders.
- Server config can disable purchase marking globally or per trader.
- Items with a trader marker cannot be listed on the flea market by default.
- Server config can disable the flea market restriction globally or per trader.
- Client plugin shows a marker on item icons and displays the trader name in the marker tooltip.
- Client config can change marker position and color. The default marker position is top-left.
- Player-facing text is localized for SPT-supported languages including English, Chinese, French, German, Italian, Japanese, Korean, Polish, Portuguese, Russian, Spanish, and Turkish. English is the default; Auto follows the detected game language when possible.

## Build

The server project depends on SPTarkov NuGet packages `4.0.13`.

The client project builds a real BepInEx plugin with NuGet reference assemblies by default. You can also set `SPTPath` to your SPT installation directory so the build prefers local SPT client assemblies:

```powershell
dotnet build .\MoeTradeMarker.sln -p:SPTPath=C:\SPT
```

If Moe-TradeMarker does not appear in the F12 `plugin / mod settings` menu, the installed client DLL is usually not the real `[BepInPlugin]` plugin DLL, or it was not placed under `BepInEx/plugins/Moe-TradeMarker/`.

## Configuration

The server config file is `Server/Config/config.json`; it is copied to the server mod output directory during build.

Client settings are available through the BepInEx configuration menu:

- `ShowTraderMarker`: show the trader marker.
- `MarkerPosition`: `LeftTop`, `RightTop`, `LeftBottom`, `RightBottom`.
- `MarkerColor`: marker icon color, light cyan by default.
- `LanguageMode`: `Auto` follows the detected game language; manual values use SPT-supported language codes.
