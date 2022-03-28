using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class MoveLeaf : Leaf<MoveLeafData>, ILeafMemberDicSetter
    {
        private Transform transform;

        public override void OnReciveDataFinish()
        {
            
        }


        public void SetMemberDic(LeafMemberDic dic)
        {
            transform = dic[MemberKind.mover] as Transform;
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

