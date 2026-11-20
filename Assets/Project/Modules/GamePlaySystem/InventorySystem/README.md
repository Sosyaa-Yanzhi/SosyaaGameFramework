# Inventory 系统说明

## 系统职责

Inventory 系统负责 UI 网格容器中的物品拖拽、网格匹配、放置和占用状态管理。运行时主要由以下组件协作：

| 组件 | 职责 |
| --- | --- |
| `InventoryManager` | 中介拖拽状态及库存指针进入/离开通知，保存当前拖拽物品和悬停容器。 |
| `Inventory` | 保存容器网格，并将指针事件转发给 `InventoryManager`。 |
| `InventorySlot` | 表示一个逻辑网格，保存坐标和占用状态 `bIsSlotMatched`。 |
| `Item` | 保存物品自身网格与锚点，搜索可放置位置、检查物品 footprint，并完成吸附。 |
| `InventoryItemManager` | 维护已放入容器的物品列表。 |
| `InteractableComp` | 转发拖拽开始、拖动、结束事件，并移动物品 UI。 |

## 网格坐标与匹配规则

- 容器及物品网格通过 `InventorySlot.slotPosition` 使用逻辑坐标。
- 每个物品应在 Inspector 中将 `Item.pivotSlot` 指向该物品的锚点网格；锚点逻辑坐标应为 `(0, 0)`。
- `Item` 在启动时收集自身及子级中的 `InventorySlot` 作为物品形状网格。
- `Inventory` 在 `Awake` 时收集其子级中的 `InventorySlot` 作为容器网格。
- `Item.detectDistance` 限制锚点候选的搜索距离。距离在共同父级的局部坐标系中计算，不需要临时更改网格父级。
- 一个候选位置只有在物品的每个网格都能映射到容器中的对应网格，且目标格未被占用时，才算可放置。
- 多个候选均有效时，选择距离锚点最近的候选。

## 拖拽与放置流程

1. `InteractableComp` 收到拖拽事件后通知 `Item`。
2. `Item` 在拖拽开始时释放原占用，并通过 `InventoryManager.BeginItemDrag` 登记当前物品。
3. 指针进入或拖动经过容器时，`Inventory` 通知 `InventoryManager`；管理器再通知当前 `Item` 更新悬停容器和候选位置。
4. `Item` 持续记录最近的可放置位置。拖拽结束时，将物品吸附到记录的锚点网格。
5. 放置成功后，物品自身网格及 footprint 覆盖的所有容器网格设为已占用，并将物品加入 `InventoryItemManager.itemsInInventory`。
6. 拖拽结束后，`Item` 通知 `InventoryManager.EndItemDrag` 清理拖拽状态。若没有有效位置，则跳过吸附。

## 状态说明

- `Item.bIsInInventory` 表示物品当前处于某个容器的指针范围内。
- `Item.bIsPut` 表示物品已成功放置。
- `InventorySlot.bIsSlotMatched` 表示网格已被占用/匹配。
- 拖拽物品离开原放置状态时，会清除该物品记录的容器网格及物品自身网格占用；成功放置后再按新 footprint 设置占用状态。
- `InventoryManager` 管理当前拖拽物品、拖拽状态及当前悬停容器，避免 `Inventory` 直接修改 `Item` 的状态。

## 场景配置

- 场景需要启用 `EventSystem`，Canvas 需要配置 `GraphicRaycaster`。
- 容器的可交互区域需要可被 UI 射线命中的 Graphic；若希望空隙区域也能触发指针事件，可在容器根节点添加透明 Image 并启用 Raycast Target。
- 物品的 `CanvasGroup.blocksRaycasts` 应在非拖拽时保持原设置。若物品配置了 CanvasGroup，`InteractableComp` 会在拖拽期间临时关闭 `blocksRaycasts`，结束后恢复原值。
- 物品上的 `pivotSlot` 必须属于该物品的网格层级；若锚点缺失或不属于物品，启动时会记录错误。

## 修改记录 - 2026.11.20

- 将拖拽开始/结束和容器指针进入/离开状态通过 `InventoryManager` 中介。
- 修复 RectTransform 距离计算中的临时重设父级，改为转换到共同父级坐标系计算。
- 搜索并记录最近的可匹配网格；在指针进入容器和拖动过程中刷新位置记录。
- 校验物品完整 footprint，防止部分网格落在容器范围外仍被放置。
- 放置成功时更新物品及所有覆盖容器网格的占用状态；拖拽取出时释放原占用，并拒绝占用冲突位置。
- 为无效容器、网格和拖拽目标增加保护，保证拖拽结束时清理管理器状态。
