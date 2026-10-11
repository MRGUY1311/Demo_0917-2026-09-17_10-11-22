
# Vertical Slice v0

> 目标：制作一个 **3D 俯视角近战 Boss Duel**，用于验证 Gameplay Programmer 的完整工作闭环。  
> 当前清单是 **初版范围**，实际开发中允许迭代、替换或删除功能。  
> 核心原则：先做出可玩的战斗闭环，再根据实际体验决定是否增加辅助功能。

## 清单使用与阶段安排

最近核对：2026-10-11，依据提交基线 `97645df` 及 10/09–10/11 保存的代码、配置。当前为动作管理器迁移检查点：用户反馈 Locomotion ↔ Dash 验收通过，LightAttack 为零时长占位，DashAttack 请求暂时屏蔽。历史勾选保留迁移前已成立的成果，不代表新链路全部回归；本次收尾没有由助手进行 Play 或构建验收。

- `[x]`：该事项描述的最小版本已经成立；后续手感与稳定性验收仍可发现问题。
- `[ ]`：尚未完成；注明“部分完成”的事项保留已有成果与剩余缺口。
- “候选 / 可选”：按实际需求决定是否采用，不自动成为 v0 冻结的阻塞项。
- 开发顺序：自然实现 → 战斗闭环 → Gameplay 打磨与回归 → 冻结自然基线 → 工业化回顾与选择性重构。
- 当前架构修改以已暴露的动作派生、输入窗口和生命周期问题为依据。
- 原安排为 2026 年 12 月上旬完成；当前目标为 11 月 15 日完成垂直切片与 Gameplay 打磨，最晚目标为 11 月 30 日。双项目工时与试行里程碑统一维护在 [共用日程](Project_Schedule.md)，根据实际执行与最终 Ability 范围迭代；工业化回顾单独排期。

### 排期前待决定

- [ ] 根据 [共用日程](Project_Schedule.md) 的 Demo 每周 13–14h 试行目标，记录学习、实现、调试、验收的实际耗时并校准
- [ ] 确定 DashAttack 是否计入原定的 2 个主动 Ability，还是作为 Dash 派生单独保留
- [ ] 根据工时与范围确定战斗闭环、v0 冻结和工业化回顾的里程碑日期

## 1. 核心体验

- [ ] 单房间 Boss / 精英战
- [ ] 单局时长约 3–5 分钟
- [ ] 核心循环：读招 → 位移 → 找攻击窗口 → 输出 → 脱离
- [ ] 不依赖 Build / Roguelike 成长系统
- [ ] 战斗本身能够支撑重复游玩和调试

## 2. 玩家控制

### 基础移动

- [x] WASD 相机相对自由移动（Input System + CharacterController）
- [x] 鼠标世界位置决定攻击 Aim；非攻击状态 Facing 采样移动方向
- [x] Movement 与 Facing 解耦，PlayerFacing 为角色旋转的唯一写入者
- [x] Dash 保留 Mouse Aim / Facing 两种方向模式
- [x] 攻击开始时采样攻击方向
- [x] 当前攻击通过 Facing Lock 保留方向 commitment
- [x] 俯视角相机平滑跟随

> 原方案“持续鼠标 Facing、Dash 优先 Move Input”已发生迭代。当前非攻击 Facing 跟随 Move；Dash 的 Facing 模式不等同于直接采样当前 Move Input。

### 后续迭代候选
- [ ] Soft Aim Assist
  - [ ] 攻击启动时检测 Boss 是否位于辅助角度 / 距离内
  - [ ] 对 Raw Aim 做有限修正
  - [ ] 对比 Aim Assist OFF / ON 的手感
- [ ] Strafe 动画表现优化
- [ ] 攻击启动阶段有限转向
- [ ] 手柄输入支持

## 3. 玩家战斗

### 基础攻击

- [x] 3-hit Light Combo
  - [x] Light A
  - [x] Light B
  - [x] Light C
- [x] Combo 输入窗口（Active / Recover）
- [x] 连段窗口内最多缓存一次下一段输入
- [x] Attack Startup / Active / Recovery
- [x] Locomotion Blend Tree 与轻击返回路径
- [ ] 按实际手感决定跨 Action 的输入缓存、过期和消费规则
- [ ] 明确轻攻击的转向政策：当前每段起手采样并锁向，持续转向尚未实现
- [ ] 挥空与命中后的手感对比（在 Gameplay 打磨阶段验收）

### Dash

- [x] 任意方向程序位移 Dash，使用 Roll 动画表现
- [x] Dash 与 Attack 输入仅响应 performed
- [ ] Dash i-frame
- [ ] 修复抵着 Cube 翻滚时偶发沿墙缓慢位移，翻滚结束后仍持续滑动一段时间（2026-10-09 已定位为 MoveRoutine；新版 Tick 已接入侧碰停止，贴墙回归待确认）
- [ ] Dash 与攻击状态的取消规则（当前 DashAttack 为预约后衔接，不在按键时取消 Dash；统一交接与清理待完成）
- [x] Dash Attack 最小版本：Dash 期间预约，程序 Dash 结束后起手并接入伤害；动画与计时边界仍待专门验收
  - [x] 为测试在 Dash 的 Startup / Active / Recover 阶段开放输入窗口
  - [x] PendingDashAttack bool 缓存一次请求，Animator 使用该参数区分攻击与返回 Locomotion 的分支
  - [x] 不在按键时停止 Dash 位移；程序 Dash 结束后进入独立 DashAttack Action，起手采样方向并锁定 Facing
  - [x] Active 入口调用共享命中，正常结束恢复移动与 Facing，并清除 PendingDashAttack
- [ ] 根据战斗体验决定 Dash 冷却或使用节奏
- [ ] 比较 Root Motion 与程序位移的适用性（学习候选，当前使用程序位移）

### Ability A — Leaping Strike
定位：Gap Closer / 接近工具

- [ ] 中距离突进攻击
- [ ] 攻击方向与位移方向处理
- [ ] 目标距离过近 / 过远时的行为
- [ ] 碰撞处理
- [ ] 可否被受击打断
- [ ] Root Motion / 程序位移对比

### Ability B — Heavy Flourish
定位：高风险、高收益、大范围攻击

- [ ] 长 Startup
- [ ] 较大攻击范围
- [ ] 高 Damage
- [ ] 高 Poise Damage
- [ ] 较长 Recovery
- [ ] 与轻击形成明显功能差异

## 4. 战斗基础系统

- [x] Hitbox 最小版本：瞬时 Sphere Query
- [x] Hurtbox 接收命中并转交 Health
- [x] Damage
- [x] HP
- [ ] Hit Reaction
- [x] 死亡标记与回调入口（当前仅日志，并拒绝后续伤害）
- [ ] 完整 Death：停止行为、清理动作、表现死亡并进入胜负流程
- [ ] Restart

### 近期重构
- [ ] 重构通用命中与攻击边界
  - [x] 提取 HitBox / HitData / HurtBox 命中链路
  - [x] 保持各 Action 负责时序、动画与状态；Hitbox 只负责空间判定与命中转交
  - [ ] 第一版维持瞬时 Sphere Query，并保证一次判定内目标不会重复受击
- [ ] 收拢 Action 生命周期与派生规则
  - [x] PlayerActionState 暴露只读 currentAction / currentPhase
  - [x] PlayerActionController 接管普通 Dash，输入与 Movement 使用同一权限来源（2026-10-11 用户反馈 Locomotion ↔ Dash 验收通过）
  - [ ] 将 LightAttack 与 DashAttack 迁入完整行为生命周期，恢复并验收派生请求
  - [ ] 增加 Dash → DashAttack 的原子 TryTransition
  - [ ] 统一完成与取消的清理路径，停止具体 Coroutine 而非 StopAllCoroutines
  - [x] DashAttack 输入许可由 Action / Phase 推导，移除 canDashAttack 字段；PendingDashAttack 保留为请求缓存而非权限
  - [ ] 将 Attack 输入收成单一入口，明确派生动作优先级
- [x] 结构化共享阶段数据（实现与存量参数迁移完成，待 Play 回归）
  - [x] 提取 ActionTiming（Startup / Active / Recovery / TotalDuration）
  - [x] AttackSegment 与 DashSettings 组合 ActionTiming，移除 dashDuration / 3
  - [x] 将共享 AttackSegment 移出 PlayerAttack.cs
- [ ] DashAttack 收尾
  - [x] Recovery 的 owner 修正为 DashAttack
  - [x] Active 入口调用共享 HitBox.ResolveHit，增加 AttackSegment 转交 HitData 的重载
  - [ ] 开始失败、取消或后续死亡 / Restart 时清除 PendingDashAttack，避免残留到下一次 Dash
  - [ ] 对比动画起手 / 命中与程序阶段计时，验收窗口末端输入、方向变化及碰墙时序
  - [ ] 为错误 owner 的 SetPhase / TryEnd 增加可见诊断

### 时间与手感

- [x] Startup / Active / Recovery 最小版本
- [x] Light Combo 单次输入缓存
- [ ] 跨动作 Input Buffer 规则与边界验收（需求驱动）
- [ ] Cancel Window 与派生优先级验收
- [ ] Hitstop
- [ ] i-frame
- [x] 起手方向采样与攻击期间锁向
- [ ] 根据不同 Action 调整 Attack Commitment

## 5. Boss

### 基础定位
- [ ] 单一 Boss / 精英敌人
- [ ] 重甲近战型
- [ ] 动作速度整体慢于玩家
- [ ] 攻击范围与单次威胁高于玩家
- [ ] 通过明显前摇支持读招

### 初版招式

冻结前明确少量招式的组合与数量；下列项目保留为初版设计候选。

- [ ] 2–3 hit Combo
- [ ] Delayed Heavy Stab
- [ ] Leaping Strike
- [ ] Heavy Flourish

### Boss 行为
- [ ] 基础距离判断
- [ ] 攻击选择
- [ ] 攻击间隔
- [ ] 追击 / 接近
- [ ] 受击
- [ ] Stagger
- [ ] Death

## 6. Poise / Stagger

初版优先只给 Boss 使用。

- [ ] Boss Poise
- [ ] 攻击具有 Poise Damage
- [ ] 轻攻击造成少量 Poise Damage
- [ ] Heavy Flourish 造成高 Poise Damage
- [ ] Poise 清空触发 Stagger
- [ ] Stagger 是否打断 Boss 当前攻击
- [ ] Boss 部分攻击阶段可考虑 Super Armor
- [ ] Poise 恢复规则

## 7. Boss Phase 2

原则：**重组已有行为，不增加新的大型系统。**

- [ ] 50% HP 左右进入 Phase 2
- [ ] 调整攻击间隔 / 攻击欲望
- [ ] 组合已有招式
  - [ ] Combo → Heavy Flourish
  - [ ] Leap → Combo
- [ ] 改变部分攻击延迟
- [ ] 不新增小怪
- [ ] 不新增场地机制
- [ ] 不新增大量特效依赖

## 8. Combat Feedback

初版可先使用占位效果，后续逐步迭代。基础命中、受击与 Telegraph 可读性属于 v0；具体反馈手段按体验选择，不要求同时实现下列所有特效。

- [ ] Hitstop
- [ ] Hit Flash
- [ ] Slash VFX
- [ ] Impact VFX
- [ ] Dash Trail
- [ ] 简单 Camera Shake
- [ ] 基础攻击音效
- [ ] Boss 攻击 Telegraph

## 9. 场景

- [x] 平面测试场与玩家 / 受击目标
- [ ] 单一正式 Arena 与完整战斗入口
- [x] 基本平坦
- [ ] 边界明确
- [ ] 不依赖复杂地形
- [ ] 不依赖跳跃 / 高低差
- [ ] 美术素材优先服务战斗可读性

## 10. 当前候选扩展

只有在实际开发中出现明确需求时再加入。

- [ ] Soft Aim Assist
- [ ] Block
- [ ] Parry
- [ ] Counter
- [ ] 玩家 Poise
- [ ] Knockdown
- [ ] Camera 特殊行为
- [ ] Arena 障碍
- [ ] 更复杂 Combo 派生
- [ ] 多敌人

## 11. 明确不做

- [ ] Hard Lock / Target Switching
- [ ] Roguelike Build
- [ ] 随机奖励
- [ ] 装备系统
- [ ] Inventory
- [ ] 商店
- [ ] 技能树
- [ ] 多关卡
- [ ] 随机地图
- [ ] 剧情系统
- [ ] 多 Boss
- [ ] 大量敌人生态
- [ ] 开放探索

## 12. 当前资产

### 已导入
- [x] Synty POLYGON Dungeon
- [x] Synty ANIMATION - Base Locomotion
- [x] Synty ANIMATION - Sword Combat

### 当前可直接利用

- [x] Hero Knight 精简 Prefab 作为当前玩家模型
- [x] Skeleton Knight 精简 Prefab 作为当前受击目标 / Boss 候选
- [ ] Dungeon 环境作为 Arena
- [x] Idle / Move 动画与 Locomotion Blend Tree
- [x] Light Combo A / B / C 动画
- [ ] Heavy Attack / Heavy Flourish 动画
- [ ] Leaping Strike 动画
- [x] Roll Dash 动画
- [x] DashAttack HeavyStab Hit 动画候选已整理并接入 Animator
- [ ] Hit / Stagger / Death 动画

## 13. v0 完成标准

v0 不要求“工业化”或作品集级包装，只要求形成完整的自然开发基线。

- [ ] 能进入 Arena
- [x] 玩家可自由移动和攻击
- [ ] 玩家拥有 Dash + 2 个 Ability
- [ ] Boss 能完成基本战斗循环
- [ ] 战斗可以胜利 / 失败
- [ ] Boss 至少有一次阶段变化
- [ ] 基础受击反馈存在
- [ ] 能快速 Restart
- [x] 已出现输入阶段、方向缓存、动画时序、动作所有权与派生窗口等问题，可供后续复盘

## 14. 开发原则

- 不因为“资源里有”就实现某个系统。
- 不因为“工业项目应该有”就在 v0 强制加入。
- 先形成自然实现，再在后续阶段与 JD / 行业实践对照。
- 功能只要能明显增加 Gameplay 验证价值，才进入当前范围。
- 所有参数和机制都允许在实际开发中被推翻。
- 当玩法已经能够回答职业验证问题时，优先冻结 v0，而不是无限打磨。

## 15. v0 Gameplay 打磨与冻结

开始集中打磨的条件：已经可以从战斗开始走到胜利或失败，并通过 Restart 重复体验。此前允许修复阻塞问题，并加入读招、受击所需的基础反馈。

### 战斗手感与可读性

- [ ] 对齐动画与 Gameplay 的起手、命中、恢复、移动解锁时刻，检查多余等待与突变
- [ ] 对比轻连段、DashAttack 与最终保留的 Ability 的用途、风险、伤害和使用节奏
- [ ] 验收命中位置、攻击方向与模型朝向一致；鼠标距离变化不意外改变攻击范围
- [ ] 根据连续战斗体验调整输入缓存、取消窗口、转向和 i-frame
- [ ] 使用第 8 节中选定的基础反馈区分挥空、命中、受击、Stagger 与死亡
- [ ] 检查 Boss 前摇、命中范围、追击与 Phase 2 是否可读、可躲、有反击窗口
- [ ] 对照 3–5 分钟体验目标调整双方 HP、伤害、招式间隔与战斗节奏

### 手工回归与交付

- [ ] 回归 Idle / Move / Light A-B-C / Dash / DashAttack / Ability 的进入、结束与允许迁移
- [ ] 检查连按、长按、松手、窗口边界、零方向、多 Collider 与碰墙场景
- [ ] 检查受击、Stagger、死亡、Restart 时旧协程、缓存、Facing 锁和 Action 所有权能正确清理
- [ ] 重复完成胜利、失败与 Restart；没有阻塞流程的错误或状态卡死
- [ ] 检查独立构建中的输入、动画、场景入口与重开流程
- [ ] 记录剩余非阻塞问题、保留的玩法决策与本次自然开发经历
- [ ] 保存可运行的 v0 tag / 提交、简短演示与运行说明，冻结功能范围

冻结标准：第 13 节成立，战斗有基础反馈，关键手工回归通过。候选扩展与特效数量不作为额外冻结门槛。

## 16. Post-v0 工业化回顾与选择性重构

在冻结的自然基线上单独进行。每项改造先说明实际问题、预期收益与代价，再决定是否实施；验收保留既有 Gameplay 行为。

- [ ] 保存并保留自然基线，从关键 Bug 与设计变化整理系统交互和依赖关系
- [ ] 对照届时的目标岗位 JD / 实践，选择对职业验证有价值的改进项
- [ ] 回顾 Input、Action、Animation、Hit、Health 的职责、生命周期与依赖方向
- [ ] 回顾组件初始化、配置验证、错误诊断与数据保存方式，选择必要的局部改造
- [ ] 用 Profiler 记录战斗中的 CPU / GC / 动画 / 物理查询基线，根据证据选择优化项
- [ ] 从复现过的 Bug 中选择有价值的测试或调试辅助，记录人工复现与验证步骤
- [ ] 为选择实施的重构逐项记录“问题 → 方案 → 行为回归 → 收益 / 代价”
- [ ] 整理前后对比、README 与职业判断：Gameplay 是否继续投入、Client 还缺哪些能力

本阶段不预设引入 EventBus、DI、ECS、通用 Ability Framework 或完整自动测试体系。

## 17. 迭代记录

> 每次出现明显设计变化时记录一句原因即可。

### 2026-10-08

- 变更：依据 `c96188e` 更新完成状态；补充 v0 Gameplay 打磨、手工回归、冻结与 Post-v0 工业化回顾清单。
- 原因：需要保留自然开发基线，明确剩余工作与打磨出口，避免把工程化改造计入无限延伸的 v0。
- 结果：DashAttack 伤害、动作生命周期与共享阶段数据仍列为近期待办；里程碑待实际工时与 Ability 范围讨论后确定。

#### 当日开发收尾

- 变更：DashAttack 接入共享命中；Mouse Aim 单位化；Action / Phase 对外只读；PendingDashAttack 缓存一次输入，程序 Dash 结束后启动攻击，并由 Animator 的 bool 条件选择退出分支。
- 原因：立即停止协程会造成翻滚中停位移与转向；延后发送旧 Trigger 又可能错过 Dash 的过渡并残留到下一次。
- 结果：用户反馈当前衔接效果尚可，保留调试注释与自然迭代痕迹；失败清理、动画 / 程序时序、窗口末端和碰墙场景仍列入后续验收，不视为完整回归已通过。
- 安排：新增双项目共用日程与 Obsidian 同步副本；v0 与 Gameplay 打磨目标为 11/15，最晚目标为 11/30，工业化回顾单独排期。

### 2026-10-09

- Bug 记录：抵着 Cube 翻滚时，有概率在翻滚中沿墙缓慢位移；翻滚结束后仍持续上述位移一段时间。暂未确认是否伴随持续 WASD 输入、具体入射角及 DashAttack 预约。
- 静态定位线索：Dash 的状态协程按时长结束，MoveRoutine 却按实际累计位移达到 dashDistance 才结束；贴墙后若每帧仍有大于停止阈值的小位移，位移协程可能超过 Dash 的时长继续运行，EndDash 当前也未停止它。
- 待验证：松开 WASD 后复现，核对残余位移时的 Action / Phase 及 MoveRoutine 是否仍在调用 Move；比较正面抵墙、斜向擦墙及有 / 无 DashAttack 预约。
- 原始记录状态：仅记录用户现象与未运行确认的原因假设，没有修改 Dash / Movement 脚本。
- 后续定位：用户确认是 MoveRoutine 的问题；当前用户代码已加入 CollisionFlags.Sides 碰撞退出与 EndDash 停止位移协程。本次提取完整保留这部分改动，尚未收到修改后的 Play 回归结果。
- 数据提取：新增独立 ActionTiming、AttackSegment、DashSettings；轻攻击 A/B/C 与 DashAttack 改为读取 timing，Dash 不再通过总时长三等分推进阶段。它们仍是内联序列化数据，不负责动作执行或状态管理。
- 参数迁移：SampleScene 中四段攻击的阶段时长及 HitData 原样保留；Dash 距离保留 5，原总时长 0.8 的三等分结果各迁移为 0.26666668。逐项静态比对通过，未编译或进行 Play 验收。
- 设计决定：按用户要求暂不补齐局部 fallback；离散的动作派生、生命周期管理与角色方向写入留到后续设计讨论，本次没有改动这些行为。

### 2026-10-10

- 变更：用户继续实现 PlayerActionController，新增空请求 / 空时长常量、退出原因、统一退出清理及阶段入口调用；Dash.GetTiming 已返回现有普通 Dash 参数，接口 Exit 已带退出原因。
- 当前状态：其他动作行为入口仍有 NotImplementedException；CanEnd 为占位实现，CanBegin 尚未检查动作占用，组件引用初始化与新旧链路接线仍待完成。不将此草稿视为可运行迁移已验收。
- 明早先核对：activeEnd 应使用 Startup + Active 的累计边界；拒绝决策已移到 ActionDecision，但 AnalyzeAttack 仍使用未限定的 rejection。助手只记录静态检查线索，没有改脚本或编译。
- 下一次执行节点：新控制器独立驱动普通 Dash，正常或碰墙后按阶段结束，恢复移动且可再次 Dash；优先补权限与行为入口，再接通输入和 Movement 的同一权限来源。
- 范围与时间：用户计划 10/11 上午至午餐前继续，不自动扩展到下午 / 晚间；暂不迁移完整 DashAttack、轻攻击连段或重做动画 / Facing。

### 2026-10-11

- 变更：新增 IPlayerActionBehavior 与动作请求 / 决策结构；PlayerActionController 负责请求分析、单槽预约、阶段计时和退出清理，初始化复用 TryRequireComponent。Dash 通过 Enter 保存执行请求，通过阶段入口驱动动画，通过 Tick 执行程序位移并在侧碰后停止位移。
- 接线：Movement 改为查询 PlayerActionController.GetMoveAccess；保存的 SampleScene 将 Attack / Dash 事件汇总到管理器，玩家节点改挂新管理器，存量攻击与 Dash 的阶段参数沿用原值。保留 Dash_、旧状态脚本与早期伪代码作为迁移参考。
- 占位：PlayerAttack 的 GetTiming 返回 ActionTiming.Empty，其余行为接口为空，旧输入入口暂不启动攻击协程；管理器暂时忽略 Dash / DashAttack 期间的攻击输入，避免普通 Dash 验收触发未完成派生。暂时不播放轻攻击动画、转向或造成命中。
- 验收：用户反馈 Locomotion ↔ Dash 没有问题；未据此标记两种方向的全部边界、贴墙、攻击派生或完整战斗回归已通过。助手本次没有进行 Play 或构建验收。
- 未完成：Dash → DashAttack 尚不能执行；已有一个明确阻断是 OnAttack 中的临时保护。保存版本还保留 activeEnd 累计边界、CanBegin 忙碌检查、CanPend 条件括号、方向归一化及 derive 退出清理等迁移缺口，不在提交时顺手改动。
- 下次入口：2026-10-12 中午先恢复 DashAttack 请求入口，再检查请求缓存、动作交接与动画 / 判定衔接；LightAttack 继续占位。今天到此收工。

### YYYY-MM-DD
- 变更：
- 原因：
- 结果：
