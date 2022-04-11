using System;
using XLua;

namespace NullFramework.Runtime
{
    public class LuaLeaf<T> : LuaLeaf, ILeafDataReciver where T : LeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() {}

        public virtual void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
        }
    }

    [LuaCallCSharp]
    public class LuaLeaf : Leaf
    {
        private static LuaEnv envCache;
        private static LuaEnv luaEnv
        {
            get
            {
                if(envCache == null)
                {
                    envCache = new LuaEnv();  
                    envCache.AddLoader((ref string file) =>
                    {   
                        var bytes = LRes.LoadLuaBytes(file);
                        return bytes;
                    });
                }
                return envCache;
            }
        }
        public bool IsFinish;
        public bool IsStop;
        private LuaTable scriptEnv;
        private Action luaExecute;
        

        public LuaLeaf() : base()
        {
            
        }

        public void SetLuaCodeByFile(string lauFile)
        {
            string code = $"require '{lauFile}'";
            SetLuaCode(code, lauFile);
        }

        public void SetLuaCode(string code, string chunkName = "chunk")
        {
            var luaBytes = System.Text.Encoding.UTF8.GetBytes(code);
            if(scriptEnv != null) 
            {
                scriptEnv.Dispose();
                scriptEnv = null;
            }
            scriptEnv = luaEnv.NewTable();
            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            luaEnv.DoString(luaBytes, chunkName, scriptEnv);
            luaExecute = scriptEnv.Get<Action>("execute");

        }

        public void ExecuteLuaScript()
        {
            
            scriptEnv.Set("leaf", this);
            luaExecute?.Invoke();
        }

        public void AddMsg(Msg msg)
        {
            root.AddMsg(msg);
        }
      
        
    }
}
