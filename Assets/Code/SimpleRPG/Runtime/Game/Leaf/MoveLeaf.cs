using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class MoveLeaf : Leaf<MoveLeafData>
    {
    
        protected override void InitListeners()
        {
            AddMsgListener(GameMsgKind.Move, Move);
        }

        private System.Action Move(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Move>();
            var mover = msgData.mover;
            if(!mover)
            {
                return null;
            }
            var speed = leafData.MoveSpeed * msgData.strength * msgData.dir;
            var dis = speed * Root.Instance.DeltaTime;
            mover.Translate(dis, Space.World);
            return null;
        }
        
    }
}

