using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class MoveLeaf : Leaf<MoveLeafData>, ILeafReciver
    {
        private Transform transform;

        public override void OnReciveDataFinish()
        {
            
        }

        public void ReciveLeaf(Leaf leaf)
        {
            if(leaf is PlayerTress playerTress)
            {
                this.transform = playerTress.Player.transform;
            }
        }

        public void SetUnityObject(Object obj)
        {
            this.transform = (obj as GameObject)?.transform;
        }

        protected override void InitListeners()
        {
            AddMsgListener(GameMsgKind.move, Move);
        }

        private void Move(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Move>();
            
            var speed = leafData.MoveSpeed * msgData.strength * msgData.dir;
            var dis = speed * Root.Instance.DeltaTime;
            this.transform.Translate(dis, Space.World);
        }
        
    }
}

