using System;
using System.Collections.Generic;
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
                    envCache.DoString("require 'start_env'");
                    luaExecuteAction = envCache.Global.Get<Action>("Execute");
                }
                return envCache;
            }
        }
        
        private static Action luaExecuteAction;
        private const string key_guid = "Guid";
        private const string key_classId = "ClassId";
        protected const string key_data = "Data";
        private static Dictionary<string, int> luaClassDic = new Dictionary<string, int>();

        public static void SaveLuaClass(string filename, int calssId)
        {
        #if UNITY_EDITOR
            if(luaClassDic.ContainsKey(filename))
            {
                UnityEngine.Debug.LogError($"{filename} 存在重名");
            }
        #endif
            luaClassDic[filename] = calssId;
        }

        private static int LoadLuaClassId(string filename)
        {
            return luaClassDic[filename];
        }

        public bool IsFinish;
        public bool IsStop;
        private LuaTable scriptEnv;
        //先拿guid凑活 之后换成int
        private string guid;
        private int calssId;
        //lua代码执行结果
        protected int excuteState;
        
        

        public LuaLeaf() : base()
        {
            
        }

        public void SetLuaCodeByFile(string luaFile)
        {
            string code = $"require '{luaFile}'";
            SetLuaCode(code, luaFile);
            calssId = LoadLuaClassId(luaFile);
            guid = System.Guid.NewGuid().ToString();
        }

        private void SetLuaCode(string code, string filename = "chunk")
        {
            var luaBytes = System.Text.Encoding.UTF8.GetBytes(code);
            if(scriptEnv != null) 
            {
                scriptEnv.Dispose();
                scriptEnv = null;
            }
            luaEnv.DoString(luaBytes, filename);
        }

        public int ExecuteLuaScript(MsgData data = null)
        {
            if(data != null) luaEnv.Global.Set(key_data, data);
            luaEnv.Global.Set(key_classId, calssId);
            luaEnv.Global.Set(key_guid, guid);
            luaExecuteAction?.Invoke();
            //暂时就一帧 跑了就成功
            excuteState = StateKind.Success;
            return excuteState;
        }

        public static void AddMsg(Msg msg)
        {
            root.AddMsg(msg);
        }

        public static void AddNextFrameMsg(Msg msg)
        {
            root.AddNextFrameMsg(msg);
        }
        
    }
}
