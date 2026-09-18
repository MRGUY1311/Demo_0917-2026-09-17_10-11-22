
# Vertical Slice v0

> 目标：制作一个 **3D 俯视角近战 Boss Duel**，用于验证 Gameplay Programmer 的完整工作闭环。  
> 当前清单是 **初版范围**，实际开发中允许迭代、替换或删除功能。  
> 核心原则：先做出可玩的战斗闭环，再根据实际体验决定是否增加辅助功能。

## 1. 核心体验

- [ ] 单房间 Boss / 精英战
- [ ] 单局时长约 3–5 分钟
- [ ] 核心循环：读招 → 位移 → 找攻击窗口 → 输出 → 脱离
- [ ] 不依赖 Build / Roguelike 成长系统
- [ ] 战斗本身能够支撑重复游玩和调试

## 2. 玩家控制

### 基础移动
- [ ] WASD 自由移动
- [ ] 鼠标世界位置决定朝向 / 攻击方向
- [ ] Movement 与 Facing 解耦
- [ ] Dash 按当前移动输入方向执行
- [ ] 攻击开始时采样攻击方向
- [ ] 攻击过程中保留一定方向 commitment

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
- [ ] 3-hit Light Combo
  - [ ] Light A
  - [ ] Light B
  - [ ] Light C
- [ ] Combo 输入窗口
- [ ] Input Buffer
- [ ] Attack Startup / Active / Recovery
- [ ] Dash Cancel Window
- [ ] 挥空与命中后的手感对比

### Dash
- [ ] 四方向 / 任意方向 Dash
- [ ] Dash i-frame
- [ ] Dash 与攻击状态的取消规则
- [ ] Dash 冷却或使用节奏
- [ ] 比较 Root Motion 与程序位移的适用性

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

- [ ] Hitbox
- [ ] Hurtbox
- [ ] Damage
- [ ] HP
- [ ] Hit Reaction
- [ ] Death
- [ ] Restart

### 时间与手感
- [ ] Startup / Active / Recovery
- [ ] Input Buffer
- [ ] Cancel Window
- [ ] Hitstop
- [ ] i-frame
- [ ] Attack Commitment

## 5. Boss

### 基础定位
- [ ] 单一 Boss / 精英敌人
- [ ] 重甲近战型
- [ ] 动作速度整体慢于玩家
- [ ] 攻击范围与单次威胁高于玩家
- [ ] 通过明显前摇支持读招

### 初版招式
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

初版可先使用占位效果，后续逐步迭代。

- [ ] Hitstop
- [ ] Hit Flash
- [ ] Slash VFX
- [ ] Impact VFX
- [ ] Dash Trail
- [ ] 简单 Camera Shake
- [ ] 基础攻击音效
- [ ] Boss 攻击 Telegraph

## 9. 场景

- [ ] 单一 Arena
- [ ] 基本平坦
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
- [ ] Hero Knight 作为玩家候选
- [ ] Skeleton Knight 作为 Boss 候选
- [ ] Dungeon 环境作为 Arena
- [ ] Light Combo 动画
- [ ] Heavy Attack / Heavy Flourish 动画
- [ ] Leaping Strike 动画
- [ ] Dodge 动画
- [ ] Hit / Stagger / Death 动画

## 13. v0 完成标准

v0 不要求“工业化”或作品集级包装，只要求形成完整的自然开发基线。

- [ ] 能进入 Arena
- [ ] 玩家可自由移动和攻击
- [ ] 玩家拥有 Dash + 2 个 Ability
- [ ] Boss 能完成基本战斗循环
- [ ] 战斗可以胜利 / 失败
- [ ] Boss 至少有一次阶段变化
- [ ] 基础受击反馈存在
- [ ] 能快速 Restart
- [ ] 已经出现足够多的 Gameplay 交互问题，可供后续复盘

## 14. 开发原则

- 不因为“资源里有”就实现某个系统。
- 不因为“工业项目应该有”就在 v0 强制加入。
- 先形成自然实现，再在后续阶段与 JD / 行业实践对照。
- 功能只要能明显增加 Gameplay 验证价值，才进入当前范围。
- 所有参数和机制都允许在实际开发中被推翻。
- 当玩法已经能够回答职业验证问题时，优先冻结 v0，而不是无限打磨。

## 15. 迭代记录

> 每次出现明显设计变化时记录一句原因即可。

### YYYY-MM-DD
- 变更：
- 原因：
- 结果：
