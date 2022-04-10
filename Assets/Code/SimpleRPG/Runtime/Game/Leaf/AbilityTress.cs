using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class AbilityTress : Tress<AbilityTressData>
    {
        private AbilityData abilityData;
        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            abilityData = new AbilityData(leafData);
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners
            (
                (GameMsgKind.Attack, Attack)
            );
        }
 
        private System.Action Attack(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Attack>();
            var origin = msgData.attackAbilityData;
            return () => msgData.attackAbilityData = origin;
        }
        
    }
}

