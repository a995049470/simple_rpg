
A = { val = 0 }

function A:new(o) 
    o = o or {}
    setmetatable(o, self)
    self.__index = self
    return o
end

function A:cotest()
    for i = 0, 10, 1 do
        print(i)
        self.t = i;
        coroutine.yield()
    end
end


function A:run()
    if self.co == nil then
        self.co = coroutine.create(function() A:cotest() end)
    end
    coroutine.resume(self.co)
    print(self.t)
end


aa = luaLeaf.luaCache

if aa == nil then
    aa = A:new(nil)
end

luaLeaf.luaCache = aa

aa:run()