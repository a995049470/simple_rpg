using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class SimpleRPGRoot : Root
    {
        
        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            Vector3 dir = Vector3.zero;
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                dir.z += 1;
            }
            if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                dir.z -= 1;
            }
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                dir.x -= 1;
            }
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                dir.x += 1;
            }
            if(dir.magnitude > 1) dir = dir.normalized;
            var data = new MsgData_Move();
            data.dir = dir;
            data.strength = 1;
            AddMsg(new Msg(MsgKind.move, data));
            AddMsg(new Msg(MsgKind.custom));
        }
    }
}

