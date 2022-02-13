using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class Root_SimpleRPG : Root
    {
        protected override void HandleInputEvent()
        {
            base.HandleInputEvent();
            Vector2 dir = Vector2.zero;
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                dir.y += 1;
            }
            if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                dir.y -= 1;
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
            var data = new MoveData();
            data.dir = dir;
            data.strength = 1;
            AddMsg(new Msg(MsgKind.move, data));
        }
    }
}

