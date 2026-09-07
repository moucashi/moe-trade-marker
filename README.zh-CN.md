# Moe-TradeMarker

[English README](README.md)

[Forge 主页](https://sp-mod.com/mod/2736/moe-trademarker)
[ODDBA 帖子](https://sns.oddba.cn/?p=190592)

Moe-TradeMarker 的目标很简单：让你一眼看出哪些物品是从 NPC 商人那里买来的。购买后的物品会在客户端显示小型商人角标，你可以更轻松地区分商人货、战局掉落和其他来源的物品。默认情况下，它还可以阻止带有商人标记的物品上架跳蚤市场，帮助你在 SPT 存档中更稳定地执行“商人货不可转卖”的规则。

## 你会得到什么

- NPC 商人购买的物品会显示小型商人角标。
- 物品悬浮提示会显示对应商人名称。
- 可选的跳蚤市场限制，防止商人货被转卖。
- 可以全局或按商人调整购买标记和跳蚤限制。
- 可以在 BepInEx 配置菜单中调整角标位置和颜色，新安装默认显示在左下角。
- 扩展玩家可见标记文本的多语言显示支持。
- `Auto` 语言模式会尽量跟随检测到的游戏语言。
- 可选兼容 BlackHawk QuickSell；受限物品的 `QuickSell (Flea)` 菜单会置灰。

## 安装

下载最新版本压缩包，并解压到 SPT 根目录。压缩包包含：

`BepInEx/plugins/Moe-TradeMarker/`
`SPT_Runtime/user/mods/Moe-TradeMarker/`

如果 F12 的 `plugin / mod settings` 中没有看到 Moe-TradeMarker，通常表示安装到 BepInEx 的客户端 DLL 不是带有 `[BepInPlugin]` 的真实插件，或 DLL 没有放在 `BepInEx/plugins/Moe-TradeMarker/` 下。

## 配置

安装后的服务端配置位于 `SPT_Runtime/user/mods/Moe-TradeMarker/config.json`。

客户端显示选项可在 BepInEx 配置菜单中调整：

- `ShowTraderMarker`: 是否显示商人标记。
- `MarkerPosition`: `LeftTop`、`RightTop`、`LeftBottom`、`RightBottom`，新安装默认为 `LeftBottom`。
- `MarkerColor`: 图标颜色，默认为 `1.00 0.50 0.50 1.00`。
- `LanguageMode`: `Auto` 会跟随检测到的游戏语言，也可手动选择 SPT 支持的语言。

## 兼容性

适用于 SPT 4.1.x。已针对 SPT 4.1.5 和 EFT 0.16.9.5.40743 测试。

玩家可见文本已适配 SPT 支持的语言，包括英文、中文、法语、德语、意大利语、日语、韩语、波兰语、葡萄牙语、俄语、西班牙语和土耳其语等；英文作为回退语言。

## 构建

安装 .NET 10 SDK，可与其他 SDK 并行使用。服务端使用 SPTushonka 4.1.5 依赖；客户端必须引用安装目录中的 SPT 4.1.5 / EFT 0.16.9.5.40743 实际程序集、BepInEx core、spt-common 和 Newtonsoft.Json。

```powershell
dotnet build .\MoeTradeMarker.sln -c Release -p:SPTPath=C:\SPT
.\Package.ps1 -SPTPath C:\SPT
```

普通构建不再自动生成 ZIP。Package.ps1 会在隔离目录重新构建、执行全部测试、校验程序集与插件版本，成功后生成 dist/Moe-TradeMarker-<版本>.zip；构建或测试失败不会覆盖现有压缩包。

语言选项使用各语言自称，原有 Chinese 等枚举配置值仍然兼容。配置名称随所选语言更新，Auto 跟随当前游戏语言。
