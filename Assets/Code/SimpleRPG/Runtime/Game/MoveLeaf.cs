using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class MoveLeaf : Leaf
    {
        private Transform transform;
        private float moveSpeed = 4;
        
        protected override void IntListeners()
        {
            AddMsgListener(MsgKind.move, Move);
        }

        private void Move(Msg msg)
        {
            var data = msg.GetData<MoveData>();
            var speed = moveSpeed * data.strength * data.dir;
            var dis = speed * Root.Instance.deltaTime;
        }

        
    }
}

