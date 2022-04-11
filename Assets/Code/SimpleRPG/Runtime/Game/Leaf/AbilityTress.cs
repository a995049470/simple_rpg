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
                (GameMsgKind.Attack, Attack),
                (GameMsgKind.CollectEnemy, CollectEnemy)
            );
        }
 
        private System.Action Attack(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Attack>();
            msgData.attacker.abilityData = this.abilityData;
            return null;
        }

        private System.Action CollectEnemy(Msg msg)
        {
            var msgData = msg.GetData<MsgData_CollectEnemy>();
            msgData.TryAddEnemyAblity(abilityData);
            return null;
        }

        
    }
}

