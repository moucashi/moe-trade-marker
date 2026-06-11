# Moe-TradeMarker

Moe-TradeMarker 是一个面向 SPT 4.0+ 的组合 MOD，用于标记从 NPC 商人购买的物品，并限制带有商人标记的物品上架跳蚤市场。

## 功能

- 从 NPC 商人购买物品后写入 `upd.tradeMarker.traderId`。
- 可通过服务端配置全局或按商人关闭购买打标。
- 带有商人标记的物品默认不能上架跳蚤市场。
- 可通过服务端配置全局或按商人关闭上架限制。
- 客户端在物品图标角落显示标记，并在悬浮提示中显示商人名称。
- 客户端可配置图标位置和颜色，默认显示在右上角。

## 构建

服务端项目依赖 SPTarkov NuGet 包 `4.0.13`。

客户端项目默认使用 NuGet 参考程序集构建真实 BepInEx 插件。也可以设置 `SPTPath` 指向 SPT 安装目录，让构建优先引用本地 SPT 客户端程序集：

```powershell
dotnet build .\MoeTradeMarker.sln -p:SPTPath=C:\SPT
```

如果 F12 的 `plugin / mod settings` 中没有看到 Moe-TradeMarker，通常表示安装到 BepInEx 的客户端 DLL 不是带有 `[BepInPlugin]` 的真实插件，或 DLL 没有放在 `BepInEx/plugins/Moe-TradeMarker/` 下。

## 配置

服务端配置文件位于 `Server/Config/config.json`，构建时会复制到服务端 MOD 输出目录。

客户端配置通过 BepInEx 配置菜单调整：

- `ShowTraderMarker`: 是否显示商人标记。
- `MarkerPosition`: `LeftTop`、`RightTop`、`LeftBottom`、`RightBottom`。
- `MarkerColor`: 图标颜色，默认浅青色。
