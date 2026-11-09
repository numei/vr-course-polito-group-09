# 项目结构与模块说明

## 当前实现范围

Orbital Rescue 的 A1 是桌面第一人称灰盒走查，用于确认空间尺度、舱门通行和任务区动线。任务舱、气闸、舱外平台已连通。垃圾抓取、维修判定、可交互舱门、失重移动和头显输入尚未实现。

当前没有安装或使用 ProBuilder。场景由 Unity Cube、Sphere、Cylinder 等基础几何体组成。场景对象已保存为 GameObject、Transform、MeshRenderer 和 Collider，可以直接编辑。

## 目录结构

```text
Assets/
├── Scenes/          Blockout.unity
├── Prefabs/         Player.prefab
├── Scripts/         第一人称控制器、屏幕说明、运行时程序集
├── Input/           OrbitalRescue.inputactions
├── Materials/       灰盒材质
├── Settings/        URP 渲染设置
└── Tests/
    ├── Editor/      场景与资源检查
    └── PlayMode/    控制器行为测试
Packages/            依赖清单与锁文件
ProjectSettings/     Unity 项目设置
Docs/
├── References/      原始概念 PPT、参考图片、尺寸平面图
├── Screenshots/     场景总览
├── A0-working-document.md
├── A1-checklist.md
└── Architecture.md
Tools/               批量测试脚本
```

## 1. Scenes：环境入口

`Assets/Scenes/Blockout.unity` 是主场景，已加入 Build Settings。运行时直接加载已保存的对象，不依赖场景生成脚本。

```text
Blockout
├── Environment
│   ├── 01_MissionArea
│   ├── 02_Airlock
│   └── 03_Exterior
├── Context
├── Lighting
└── Player
```

| 场景分组 | 内容与作用 | 尺度或范围 |
| --- | --- | --- |
| `01_MissionArea` | 地板、墙、顶棚、结构肋、工具架、任务终端、储物体块、观察窗占位体 | 8×12 m，z=0..12 |
| `02_Airlock` | 气闸墙体、顶棚、装备柜、控制面板和外舱门框 | 4×4 m，z=12..16 |
| `03_Exterior` | 维修柜与连接器、垃圾收集箱、静态漂浮垃圾、太阳能板、平台边界及扶手 | 12×12 m，z=16..28 |
| `Context` | 无纹理的地球球体，用于提供太空背景与尺度线索 | 远处背景，不参与碰撞 |
| `Lighting` | 一盏方向光与舱内预览点光源 | 用于看清体块，不是最终灯光方案 |
| `Player` | 玩家预制体实例 | 出生点约为 (0, 0.05, 2)，朝 +Z |

地板网格每格 1 m；两道舱门开口均为 2.4 m 宽、2.6 m 高。外部平台的围挡用于限制桌面角色的活动范围，太阳能板在平台外作为轮廓占位。垃圾当前悬停不动，没有 Rigidbody 或抓取逻辑。

修改环境时，在 Hierarchy 中选择对应分组下的体块，调整 Transform 的 Position、Rotation、Scale。移动墙体或舱门后，保留 Collider 并重新走查。装饰网格、背景地球不需要碰撞体。

## 2. Prefabs：玩家组合

`Assets/Prefabs/Player.prefab` 组织了可复用的玩家对象：

```text
Player
├── CharacterController
├── FirstPersonController
├── BlockoutGuide
└── FirstPersonCamera
    ├── Camera（MainCamera 标签）
    └── AudioListener
```

`CharacterController` 是 Unity 自带的胶囊体移动组件，高 1.8 m、半径 0.3 m、中心位于局部 y=0.9。它负责移动时与墙、地面、设备和围挡发生碰撞。它不是 Rigidbody，不会自动获得失重或太空漂移动力学。

相机的局部高度为 1.65 m，垂直视野为 75°，近裁剪面为 0.05 m。AudioListener 是场景的声音接收点，目前还没有配套音效。

修改所有玩家实例共用的速度、胶囊尺寸或相机参数，应打开 Player 预制体编辑。只想改变当前场景的出生点，则调整场景中 Player 实例的位置与朝向。

## 3. Scripts：运行逻辑

### FirstPersonController.cs

控制器连接输入动作、玩家朝向、相机俯仰和 CharacterController。

- `Awake`：记录出生位置与朝向，检查输入资产和相机引用，复制一份 InputActionAsset，查找 Gameplay 动作。每个实例独立拥有动作启停状态。
- `OnEnable`：启用动作并捕获鼠标。
- `Update`：先处理释放光标、回到出生点和恢复控制；随后处理视角与移动。
- 视角：玩家根对象绕 Y 轴旋转，相机只改变局部 X 轴俯仰，限制在 ±80°。鼠标输入是位移量，不再次乘帧时间；手柄输入是持续角速度，乘 `Time.deltaTime`。
- 移动：二维输入长度限制为 1，避免斜向加速。根据玩家朝向得到水平位移，叠加重力，再调用 `CharacterController.Move`。
- 焦点与生命周期：失去应用焦点或暂停时释放鼠标；禁用时关闭动作；销毁时释放复制的输入资产。
- 重置：暂时关闭胶囊体，恢复出生位置、朝向和相机角度，清零竖直速度，再开启胶囊体。

Inspector 中可调整的默认参数：步行 2.5 m/s、冲刺 4.5 m/s、鼠标灵敏度 0.12、手柄视角 120°/s、俯仰上限 80°、重力 -15 m/s²。

### BlockoutGuide.cs

使用 Unity IMGUI 在屏幕角落显示区域名称与按键说明。它读取控制器的 `IsCaptured` 状态，以显示“操作中”或“光标已释放”。区域名称暂时按玩家的世界 z 坐标划分：z<12 为任务舱、12≤z<16 为气闸、z≥16 为舱外。

它不控制角色、不执行任务，也不负责交互提示。如果以后改变三区的空间关系，需要同步调整区域判定，或改为触发区判定。

### OrbitalRescue.Runtime.asmdef

将运行时代码组织为 `OrbitalRescue.Runtime` 程序集，并显式引用 `Unity.InputSystem`。这是 Unity 常规的代码编译配置，不是游戏逻辑脚本。

## 4. Input：动作与设备绑定

`OrbitalRescue.inputactions` 定义 Gameplay 动作映射。编辑绑定时，在 Unity 中双击此文件。

| 动作 | 输入类型 | 绑定 |
| --- | --- | --- |
| Move | Vector2 | WASD、方向键、手柄左摇杆 |
| MouseLook | Vector2 | 鼠标 delta |
| StickLook | Vector2 | 手柄右摇杆 |
| Sprint | Button | 左/右 Shift、左摇杆按下 |
| ReleaseCursor | Button | Escape、手柄 Start |
| CaptureCursor | Button | 鼠标左键、手柄南侧按钮 |
| Reset | Button | R、手柄 Select/Back |

数据流为：设备事件 → InputActionAsset → FirstPersonController → 玩家/相机 Transform 与 CharacterController。项目设置只启用新 Input System，没有调用旧 Input Manager 的 GetAxis 或 GetKey。

## 5. Materials：灰盒识别

八个材质只提供基本颜色和明暗区分：Hull 对应舱壁，Floor 对应地面，Frame 对应结构框架，Equipment 对应设备，Identification 对应黄色识别体块，ScaleGrid 对应地板网格，DisplayPlaceholder 对应面板和太阳能板占位，EarthPlaceholder 对应地球背景。

修改共享材质会影响所有使用它的对象。当前没有最终贴图、美术模型或材质特效。

## 6. Settings、Packages、ProjectSettings：渲染与工程配置

`Assets/Settings` 来自 URP 项目模板：PC 与 Mobile 的管线资产分别引用各自 Renderer；QualitySettings 选择对应质量级别。GlobalSettings 和 VolumeProfile 保存渲染系统的默认配置。Mobile 配置保留给后续移动头显开发，但当前并没有配置可运行的 Quest/OpenXR 应用。

`Packages/manifest.json` 声明直接依赖，`packages-lock.json` 固定解析后的依赖版本。主要依赖是 URP 17.4.0、Input System 1.17.0、Test Framework 1.6.0，以及 Unity 的物理、音频、IMGUI 等基础模块。没有 ProBuilder、XR Interaction Toolkit、OpenXR 或 Cinemachine。

`ProjectSettings` 保存版本、输入模式、构建场景、渲染管线、质量级别、物理和时间设置。`InputManager.asset` 是 Unity 的标准设置文件，其存在不代表使用旧输入系统；实际使用哪套输入由 Player Settings 的 Active Input Handling 决定。

## 7. Tests 与 Tools：验证

Editor 测试检查场景与玩家相机、缺失脚本/材质、完整胶囊体通行路线及输入资产绑定。PlayMode 测试通过 Input System 的虚拟设备事件验证移动、斜向速度、碰撞、舱内外往返、鼠标视角、光标释放/恢复、重置、手柄、焦点丢失与不同帧率下的视角速度。

这些测试是维护控制器和环境的常规回归测试，不进入普通游戏运行流程。测试程序集只由 Unity Test Runner 使用。`Tools/verify.sh` 提供同一套测试的命令行入口，Unity 路径从环境变量或参数传入，报告输出到被 Git 忽略的 TestResults。

## 8. Docs 与 Unity 辅助文件

References 保留原始概念和参考图，满足 A1 的参考资料要求；Screenshots 保留一张场景总览；A0 是持续完善的概念草稿；A1 checklist 用于提交前复查。

每个资源旁的 `.meta` 文件保存稳定 GUID，场景和预制体用它建立引用。必须连同资源一起提交；移动或删除资源时也要同步处理 `.meta`。Library、Temp、Logs、UserSettings 为 Unity 本地生成数据，不属于仓库交付内容。
