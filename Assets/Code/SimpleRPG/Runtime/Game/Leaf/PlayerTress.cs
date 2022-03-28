using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class PlayerTress : Tress<PlayerTressData>, ILeafMemberDicGetter
    {
        private GameObject player;
        private LeafMemberDic membderDic;

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
    }
}


