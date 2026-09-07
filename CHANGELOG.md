# Changelog

## 1.2.6

- Keep flea selling unavailable until the initial restriction cache has loaded, without affecting trader quick selling.
- Refresh open context menus when marker data changes, including QuickSell (Flea) availability.
- Merge purchases received during a refresh without discarding updated trader names, restrictions or other items.
- Refresh open item marker tooltips after cache or language changes, preserving their original content.

- 首次限制缓存加载完成前暂时禁用跳蚤出售，不影响商人快售。
- 标记数据变化时刷新已打开的右键菜单，包括 QuickSell (Flea) 的可用状态。
- 合并刷新期间收到的购买标记，同时保留商人名称、限制和其他物品的更新。
- 缓存或语言变化后更新已打开的物品角标提示，保留原有提示内容。

## 1.2.5

- Apply server-provided purchase markers before new inventory items are displayed, without waiting for the background refresh or blocking the UI with network requests.
- Prevent older in-flight refreshes from overwriting newly received purchase markers.

- 在新购买物品显示前读取服务端返回的角标，无需等待后台刷新，也不通过网络请求阻塞界面。
- 防止购买前启动的后台刷新覆盖刚收到的新物品角标。

## 1.2.4

- Fixed marker refresh and display updates accessing destroyed Unity objects.
- Preserved refresh requests made during throttling or an active request, with non-blocking retries and atomic cache updates.
- Rejected malformed server responses without discarding the last valid marker cache.
- Fixed configuration labels following the wrong language and not updating after language changes.
- Language choices now use native names; Auto follows the active game language.
- Missing optional Show Me The Money QuickSell no longer causes patch installation errors.
- Prevented failed client builds from producing packages with stale binaries.

- 修复角标刷新和显示设置更新访问已销毁 Unity 对象的问题。
- 保留限频窗口及正在刷新期间的新请求，支持非阻塞重试与缓存整体更新。
- 拒绝非法服务端响应，保留上一份有效角标缓存。
- 修复配置名称语言错误及切换语言后未更新的问题。
- 语言列表改用各语言自称，自动模式跟随当前游戏语言。
- 未安装可选 Show Me The Money QuickSell 时不再产生补丁安装错误。
- 避免客户端构建失败后仍将旧二进制文件打入压缩包。
