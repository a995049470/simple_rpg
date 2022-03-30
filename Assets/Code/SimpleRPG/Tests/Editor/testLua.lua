local ptr
-- timer time
local function printTest()
    time = luaLeaf:PopInt()
    timer = luaLeaf:PopInt()
    print(timer..'前')
    timer = timer + 1
    print(timer..'后')
    if timer <= time then
        ptr = ptr - 1 
        luaLeaf:Push(timer)
        luaLeaf:Push(time)
        luaLeaf.IsStop = true
    end
end

local runingTable = 
{
    function() luaLeaf:Push(0) end,
    function() luaLeaf:Push(20) end,
    printTest,
}

local len = #runingTable

local function Execute()
    ptr = luaLeaf.ptr
    while luaLeaf.IsStop == false and luaLeaf.IsFinish == false do
        if ptr > len then
            luaLeaf.IsFinish = true
        else
            runingTable[ptr]()
            ptr = ptr + 1
        end
    end
    luaLeaf.ptr = ptr
end

Execute()
    
