# AI设计

## 基础敌人

| 目标 | 需要的世界状态 |
| - | - |
| 击杀玩家 | 攻击玩家 |

| actions | satisfies world state | require world state |
| - | - | - |
| 近战攻击 | 攻击玩家 | 靠近玩家 |
| | | 攻击冷却完毕 |
| 向目标移动 | 靠近玩家 |  | 
| 远离玩家 | 等待冷却 | | 

