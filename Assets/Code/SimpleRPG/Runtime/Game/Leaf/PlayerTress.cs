using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    
    public class PlayerTress : Tress<PlayerTressData>, ILeafMemberDicGetter
    {
        private GameObject player;
        private LeafMemberDic membderDic;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(GameMsgKind.custom, CustomUpdate);
        }

        public LeafMemberDic GetMemberDic()
        {
            if(membderDic == null)
            {
                membderDic = new LeafMemberDic();
                membderDic[MemberKind.mover] = player.transform;
                membderDic[MemberKind.fllowTarget] = player.transform.Find("look").gameObject;
            }
            return membderDic;
        }

        public override void OnReciveDataFinish()
        {
            player = leafData.InstantiatePlayer();
        }

        public void CustomUpdate(Msg msg)
        {
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
            Root.Instance.AddMsg(new Msg(GameMsgKind.move, data, this, this));
        }
    }
}


