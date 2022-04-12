
Msg = CS.NullFramework.Runtime.Msg
MsgData_Hit = CS.SimpleRPG.Runtime.MsgData_Hit
GameMsgKind = CS.SimpleRPG.Runtime.GameMsgKind
AddMsg = CS.NullFramework.Runtime.Root.AddMsg
AddNextFrameMsg = CS.NullFramework.Runtime.LuaLeaf.AddNextFrameMsg

ClassCache = ClassCache or {}
ClassNum = 0

ValueCache = ValueCache or {}

Guid = ''
ClassId = 0
--外部传入的Data
Data = nil

function SaveClass(c, filename)
    ClassNum = ClassNum + 1
    ClassCache[ClassNum] = c
    local id = ClassNum
    CS.NullFramework.Runtime.LuaLeaf.SaveLuaClass(filename, id)
end

function Load(guid, calssId)
    local value = ValueCache[guid] or ClassCache[calssId]:new()
    return value;
end

function Save(guid, value)
    ValueCache[guid] = value
end

function Clear(guid)
    ValueCache[guid] = nil
end

function Execute()
    local value = Load(Guid, ClassId)
    value:execute()
    Save(Guid, value)
end