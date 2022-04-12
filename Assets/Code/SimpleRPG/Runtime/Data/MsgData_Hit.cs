using NullFramework.Runtime;
namespace SimpleRPG.Runtime
{
    [XLua.LuaCallCSharp]
    public class MsgData_Hit : MsgData
    {
        public int damage;
    }

}
