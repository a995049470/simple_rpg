using System;
using XLua;

namespace NullFramework.Runtime
{
    [LuaCallCSharp]
    public class LuaLeaf : Leaf
    {
        private static LuaEnv env;
        public static LuaEnv Env
        {
            get
            {
                if(env == null)
                {
                    env = new LuaEnv();  
                    env.AddLoader((ref string file) =>
                    {   
                        var bytes = LRes.LoadLuaBytes(file);
                        return bytes;
                    });
                }
                return env;
            }
        }
        public bool IsFinish;
        public bool IsStop;
        private byte[] luaBytes;
        private LuaTable luaCache;
        

        public LuaLeaf() : base()
        {
            
        }
        public void SetLuaCode(string code)
        {
            luaBytes = System.Text.Encoding.UTF8.GetBytes(code);
            luaCache = null;
        }

        public void Execute()
        {
        #if UNITY_EDITOR
            if(luaBytes == null || luaBytes.Length == 0)
            {
                throw new Exception("没lua代码");
            }
        #endif
            IsStop = false;
            Env.Global.Set("luaLeaf", this);
            Env.DoString(luaBytes);
        }

      
        
    }
}
