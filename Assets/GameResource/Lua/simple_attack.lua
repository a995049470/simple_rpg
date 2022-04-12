
-- 开始执行普通攻击的流程

local SimpleAttack = { num = 0 }

function SimpleAttack:new(o)
    o = o or {}
    setmetatable(o, self)
    self.__index = self
    return o
end

function SimpleAttack:execute()
    local hiters = Data.hiters
    local attacker = Data.attacker
    local hiterNum = hiters.Count
    for i = 0, hiterNum - 1, 1 do
        local damage = attacker.abilityData.atk
        damage = math.max(damage - i, 0)
        local hiter = hiters[i]
        local hitData = MsgData_Hit()
        hitData.effectCount = 1
        hitData.unitObj = hiter.unitObj
        hitData.damage = damage
        local msg = Msg(GameMsgKind.Hit, hitData, attacker.leaf, hiter.leaf)
        AddNextFrameMsg(msg)
    end
end

SaveClass(SimpleAttack, 'simple_attack')






