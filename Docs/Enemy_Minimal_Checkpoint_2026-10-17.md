# 2026-10-17：敌人最小实现提交节点

前一提交基线：`346cf2b`。本次保存敌人最小链路及必要的共享动作类型迁移，作为进入游戏状态 / 暂停功能前的阶段节点。

## 已完成的最小链路

- `EnemyAI` 在玩家进入检测范围且请求间隔满足时，请求一次轻攻击；玩家引用失效后不再通过范围检测。
- `EnemyActionController` 作为敌人的唯一动作调度中心，负责动作起手、Startup / Active / Recover 推进与结束；`EnemyLightAttack` 负责动画触发、起手方向采样后的朝向请求和 Active 阶段共享命中。
- 提取共享 `ActionPhase`、`DecisionKind`、`ActionExitReason`，将 `ActionRequest<TAction>`、`ActionDecision<TAction>`、`IActionBehavior<TAction>` 泛型化。玩家使用 `PlayerAction`，敌人使用 `EnemyAction`；玩家 Dash / DashAttack / LightAttack 完成对应适配。
- SampleScene 接入 SkeletonKnight 的 AI、动作控制器、Facing、轻攻击和 Animator，配置玩家 Layer 与伤害接收组件，敌人攻击检测 Player Layer。
- `Health` 在死亡时 Destroy 所属根 GameObject；`CameraFollow` 在目标失效时停止读取其位置。当前沿用角色为独立场景根对象的前提。

## 验收与边界

- 用户于 2026-10-16 反馈敌人轻攻击最小实现验收没有问题；记录该结果，不扩展为完整 Boss、动画同步或所有死亡边界均已验收。
- 10/17 保存了玩家伤害接收、死亡销毁和相机 / 敌人引用保护相关修改。本次助手只核对提交范围、静态依赖和场景引用，没有编译、进入 Play 或构建。
- 敌人目前没有追击、复杂决策、第二种攻击或 Phase 2；不因本次提交增加这些内容。
- 动画仍保留当前自然开发配置：SkeletonKnight Controller 使用 AttackLight 进入轻攻击、Exit Time 返回 Idle；EnemyLightAttack 正常退出还会发送 AttackEnd，但该 Controller 尚无同名参数，留作后续动画接入核对，不在收尾时改动。

## 本次提交范围

包含敌人脚本 / Animator、共享类型与玩家泛型适配、伤害 / 死亡和引用保护、场景中的敌人与伤害配置，以及本节点记录。第三方模型与动画文件继续仅保存在本地。

Aim 接入 Player Map、Player.cs 的状态事件订阅、GameStateManager 草稿及移动、暂停菜单 / UI、场景中的 Manager / EventSystem / PauseMenu 和日程文件更新保留在工作区，未纳入本次敌人提交。场景通过部分暂存划分，工作区文件保持原样。

## 下次执行入口

继续 Pause ↔ InGame：先明确 GameStateManager 实例与初始化，再接通状态切换、Gameplay Map 启停和菜单显示；局部状态事件可用于变化通知，有执行顺序要求的切换步骤明确调用。

优先核对暂停期间 AI 不发新动作请求、动作阶段不继续推进，恢复后从原阶段继续；验收 Aim 在重新启用 Player Map 当帧的输入刷新边界。Restart 当前仍为草稿，随后再接通当前场景重载与时间缩放恢复。
