using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class MsgData_Move : MsgData
    {
        //角度
        public Vector3 dir;
        //强度
        public int strength;
        public Transform mover;
    }

}
