using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class MoveLeaf : Leaf<MoveLeafData>
    {
        private float moveSpeed;
        protected override void InitListeners()
        {
            AddMsgListener(GameMsgKind.Move, Move);
        }
        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            this.moveSpeed = leafData.moveSpeed;
        }

        private System.Action Move(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Move>();
            var mover = msgData.mover;
            if(!mover)
            {
                return null;
            }
            var speed = moveSpeed * msgData.strength * msgData.dir;
            var dis = speed * root.RealDeltaTime;
            mover.Translate(dis, Space.World);
            return null;
        }
        
    }
}

