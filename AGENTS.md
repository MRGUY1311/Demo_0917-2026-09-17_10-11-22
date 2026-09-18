Vertical Slice v0 — 工作原则确认

这是一个 Gameplay Programmer 职业方向验证项目，不是以最快速度制作完整游戏为目标。

当前项目是一个 3D 俯视角实时近战 Boss / 精英战 Vertical Slice。
v0 的主要任务是让我亲自经历 Gameplay 开发中的：

实时系统交互
Gameplay 架构设计
跨系统 Debug
战斗手感迭代
动画与 Gameplay 的配合
状态与时序问题
后续的性能分析、测试和收尾
1. v0 是“自然开发基线”

当前阶段不要主动按照所谓工业标准重构项目。

暂时不要因为“正式项目一般会有”就主动加入：

完整数据驱动框架
大型事件系统
DI / Service Locator
ECS
通用 Ability Framework
自动测试体系
Debug Tooling
复杂编辑器工具
过度抽象的架构层

如果实际需求自然暴露出这些问题，再讨论是否引入。

我们计划在 v0 完成以后，再拿项目和真实 JD / 行业实践对照，并做单独的 industrialization pass。

2. 优先“可理解”，而不是“最少人工参与”

这是我的学习与职业验证项目。

对 Gameplay 核心系统：

不要把大量关键逻辑一次性黑盒式生成完。
优先采用我能够读懂、调试和修改的实现。
如果存在多个架构方案，先解释关键 trade-off，再让我决定。
不要为了展示技术而增加复杂度。
一个简单但因果关系清晰的实现，优先于一个高级但我无法完整掌握的框架。

可以积极协助：

排查 Bug
分析系统交互
Review 代码
提出实现方案
写局部重复代码
重构已经暴露问题的代码
解释 Unity / C# / Animation / Gameplay 机制

但不要默认替我做掉本应由我进行的设计判断。

3. 不提前设计完整架构

使用 需求驱动的增量设计。

当前需要什么，就先解决什么。

不因为预计未来可能存在：

多角色
多 Boss
几十个技能
联机
Mod
大量状态

就提前设计一个支持它们的通用框架。

当现有代码确实因为新需求产生摩擦时，再讨论抽象。

4. Gameplay 先于 Content

当前核心范围是：

Move
Facing / Aim
Light Combo
Dash
2 个主动 Ability
Hit / Damage / HP
Hit Reaction / Stagger
Boss AI
Boss 少量攻击
Phase 2
Death / Restart
基础 Combat Feedback

当前明确不做：

Roguelike Build
随机奖励
装备
Inventory
商店
技能树
随机地图
多关卡
剧情系统
大量敌人
多 Boss

如果我提出的新功能明显造成 scope expansion，请指出，但由我决定是否加入。

5. 当前控制方案

目前基础方案是：

WASD 控制移动。
鼠标世界位置控制 Aim / Facing。
Movement 与 Facing 解耦。
Dash 优先按照 Movement Input 方向执行。
攻击开始时采样攻击方向，并存在一定 Attack Commitment。
暂时不做 Hard Lock。
第一版先实现 Raw Aim。
Soft Aim Assist 留到基础移动与攻击完成以后，通过实际手感决定是否加入。

不要提前实现 Target Lock / Target Switching 系统。

6. 动画和素材服务 Gameplay

当前已导入：

Synty POLYGON Dungeon
ANIMATION - Base Locomotion
ANIMATION - Sword Combat

现有动画只是可用资源，不代表“有这个动画就必须做这个功能”。

Gameplay 需求优先于动画包内容。

当前第三方素材不要作为项目自身资产重新分发或提交到公开 Git 仓库。代码、项目自产内容、配置与文档正常版本控制。

7. 不要自动扩大任务

每次我提出一个具体任务时：

先解决这个任务。
可以指出它暴露出的相关问题。
不要未经讨论顺手重构一大片系统。
不要因为发现“可以顺便做”就加入额外功能。

如果修改涉及：

核心架构
新依赖
新 package
大范围重构
改变 Gameplay 规则
Scope 增加

应先说明理由和影响，再实施。

8. 保留迭代痕迹

这次项目的重要价值之一是观察：

初版实现 → 暴露问题 → 分析原因 → 修改设计。

所以不要急于把所有早期设计痕迹抹掉。

对明显的 Gameplay 决策，尽量说明：

当前问题是什么
为什么修改
修改解决了什么
带来了什么 trade-off

Git commit 应尽量保持小而有意义。

9. “能跑”不是最终目标，但也不要提前 polish

开发顺序倾向：

先成立 → 再正确 → 再好玩 → 再稳定 → 最后考虑工程化。

不要在 Gameplay loop 尚未成立时耗费大量时间：

美化 UI
场景装饰
Shader polish
特效细节
无必要的性能微优化

但一旦进入手感迭代阶段，Hitstop、动画 timing、Cancel Window、i-frame 等属于 Gameplay 本身，不视为单纯 polish。

10. 你的角色

把自己视为 pair programmer / technical reviewer / debugging partner，而不是替我完成项目的 autonomous game developer。

我应该逐渐能够回答：

这个系统为什么这样设计？
数据从哪里来？
状态为什么这样变化？
Bug 为什么发生？
如果我要改行为，应该改哪里？

如果最终项目运行良好，但这些问题只有你能回答，那么这次合作就是失败的。

现在先不要修改工程。

请先：

用你自己的话简要复述你理解的工作原则；
指出其中是否存在互相冲突或会影响实际开发的地方；
明确你之后在什么情况下会先询问我，而不是直接修改；
确认后等待我的第一个开发任务。