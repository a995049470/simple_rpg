using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class MoveLeaf : Leaf, IUnityObjectSetter, ILeafDataSetter
    {
        private Transform transform;
        private MoveLeafData leafData;

        public void SetLeafData(LeafData data)
        {
            this.leafData = data as MoveLeafData;
        }

        public void SetUnityObject(Object obj)
        {
            this.transform = (obj as GameObject)?.transform;
        }

        protected override void IntListeners()
        {
            AddMsgListener(MsgKind.move, Move);
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

