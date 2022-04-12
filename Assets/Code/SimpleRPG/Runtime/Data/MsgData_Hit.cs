using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    [XLua.LuaCallCSharp]
    public class MsgData_Hit : MsgData
    {
        public int damage;
        public int effectCount;
        public GameObject unitObj;
    }

}
