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
                (GameMsgKind.CollectEnemy, CollectEnemy),
                (GameMsgKind.Hit, Hit)
            );
            AddHiddenMsgListeners(
                (GameMsgKind.Attack, Attack)
            );
        }

        private System.Action Hit(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Hit>();
            //Debug.Log($"受到 {msgData.damage} 点伤害");
            return emptyAction;
        }
 
        private System.Action Attack(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Attack>();
            if(msgData.attacker != null)
            {
                msgData.attacker.abilityData = this.abilityData;
            }
            return emptyAction;
        }

        private System.Action CollectEnemy(Msg msg)
        {
            var msgData = msg.GetData<MsgData_CollectEnemy>();
            msgData.TryAddEnemyAblity(abilityData);
            return emptyAction;
        }

        
    }
}

