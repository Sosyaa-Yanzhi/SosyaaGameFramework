# SosyaaGameFramework

一个基于 Unity 的模块化游戏框架项目，包含角色、游戏管理、存档、UI 和常见玩法系统的基础实现，供游戏开发时参考和扩展。

> 项目中的模块处于不同开发阶段，使用前请结合代码检查并按具体项目需求进行配置。对话系统目前仅有初始实现，没有使用示例，也未经测试。

## 开发环境

- Unity `2022.3.62f3c1`
- 项目依赖由 Unity Package Manager 根据 [`Packages/manifest.json`](Packages/manifest.json) 安装。

## 模块一览

| 模块 | 路径 | 内容 |
| --- | --- | --- |
| 角色 | `Assets/Project/Modules/Character` | 玩家输入、角色动作接口及动作实现；`AI` 目录目前没有实现脚本。 |
| 核心 | `Assets/Project/Modules/Core` | 事件总线、输入键及基础数据类型、可序列化字典。 |
| 管理器 | `Assets/Project/Modules/Managers` | 游戏状态、输入和场景管理相关组件。 |
| 存档 | `Assets/Project/Modules/Profile` | 存档数据映射，以及 JSON、XML、二进制格式的读写实现。 |
| 成就 | `Assets/Project/Modules/GamePlaySystem/AchievementSystem` | 成就数据和管理组件。 |
| 对话 | `Assets/Project/Modules/GamePlaySystem/DialogSystem` | 对话节点和对话管理器初始代码；未提供示例，尚未测试。 |
| 背包 | `Assets/Project/Modules/GamePlaySystem/InventorySystem` | UI 网格背包、物品拖拽、网格匹配和占用状态管理。详见[背包系统说明](Assets/Project/Modules/GamePlaySystem/InventorySystem/README.md)。 |
| 任务 | `Assets/Project/Modules/GamePlaySystem/MissionSystem` | 任务节点、任务类型数据及任务管理组件。 |
| UI | `Assets/Project/Modules/UI` | 主菜单、设置滑块、UI 配置数据和通用交互组件；部分目录保留为后续扩展位置。 |
| 编辑器工具 | `Assets/Editor` | 背包网格编辑工具和存档数据管理工具。 |

## 项目结构

```text
Assets/
├── Editor/                  # Unity 编辑器工具
└── Project/
    ├── ExampleScenes/       # Bootstrap 和 MainMenu 场景
    └── Modules/             # 框架功能模块
Packages/
└── manifest.json            # Unity Package Manager 依赖
ProjectSettings/             # Unity 项目设置
```

## 打开项目

1. 在 Unity Hub 中添加本仓库，并使用上文所列的 Unity 版本打开。
2. 等待 Unity 导入资源并解析 `Packages/manifest.json` 中的依赖。
3. 项目当前的构建场景配置为 `Assets/Project/ExampleScenes/Bootstrap.unity`。也可以在编辑器中打开 `Assets/Project/ExampleScenes/MainMenu.unity` 查看主菜单场景。
4. 使用具体模块前，检查场景中的组件、Inspector 引用和所需的 ScriptableObject 数据是否已配置。

## 使用说明

- 各模块位于 `Assets/Project/Modules` 下，可按需阅读、集成或修改。
- 背包模块的网格配置、拖拽流程和场景要求见[背包系统说明](Assets/Project/Modules/GamePlaySystem/InventorySystem/README.md)。
- 对话模块尚未提供示例或测试结果，不应视为已验证可直接使用的功能。
- 项目包含 Unity Test Framework 依赖；具体模块是否经过验证，请以对应测试和实际运行结果为准。
