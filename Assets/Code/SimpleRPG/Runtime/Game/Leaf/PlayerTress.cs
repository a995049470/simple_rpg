using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    public class PlayerTress : Tress<PlayerTressData>
    {
        private GameObject player;
        private Transform look;
    

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(GameMsgKind.Move, Move);
            AddMsgListener(GameMsgKind.FollowTarget, FollowTarget);
        }

        
        public override void OnReciveDataFinish()
        {
            player = leafData.InstantiatePlayer();
            look = player.transform.Find("look");
        }

        public System.Action Move(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Move>();
            var origin = msgData.mover;
            msgData.mover = player.transform;
            return ()=>msgData.mover = origin;
        }

        public System.Action FollowTarget(Msg msg)
        {
            var msgData = msg.GetData<MsgData_FollowTarget>();
            var origin = msgData.lookTarget;
            msgData.lookTarget = look;
            return ()=>msgData.lookTarget = origin;
        }
    }
}


