using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class UnitTress : Tress<UnitTressData>
    {
        private GameObject unitObj;
        private AbilityData abilityData;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddHiddenMsgListeners(
                (GameMsgKind.Attack, Attack),
                
                (BaseMsgKind.GoapUpdate, GoapUpdate)
            );
            AddMsgListeners(
                (GameMsgKind.CollectEnemy, CollectEnemy),
                (GameMsgKind.Collect_Launcher, Collect_Launcher),
                (GameMsgKind.Collect_Player, Collect_Player)
            );
        }

        
        public override void OnReciveDataFinish()
        {
            unitObj = leafData.InstantiateUnit();
            abilityData = leafData.CreateAblityData();
            UpdateObjectDescriptors();
        }

        protected override void UpdateObjectDescriptors()
        {
            var index = 0;
            UpdateSingleObjectDescriptor(ref index, unitObj);
            UpdateSingleObjectDescriptor(ref index, abilityData);
        }


        public System.Action Attack(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Attack>();
            //var origin_attcker = msgData.attacker;
            if((msgData.attackerFilter & abilityData.unitKind) > 0)
            {
                var battleUnit = new BattleUnit();
                battleUnit.leaf = this;
                battleUnit.unitObj = this.unitObj;
                battleUnit.unitKind = abilityData.unitKind;
                battleUnit.abilityData = this.abilityData;
                msgData.attacker = battleUnit;
            }
            return emptyAction;
            // return () => msgData.attacker = origin_attcker;
        }

        private System.Action CollectEnemy(Msg msg)
        {
            var msgData = msg.GetData<MsgData_CollectEnemy>();
            var position = unitObj.transform.position;
            msgData.TryAddEnemy(this, abilityData, unitObj, abilityData.unitKind);
            return emptyAction;
        }

        private System.Action GoapUpdate(Msg msg)
        {
            var cb = emptyAction;
            var msgdata = msg.GetData<MsgData_GoapUpdate>();
            var isPassFilter = (abilityData.unitKind & msgdata.filter) > 0;
            if(isPassFilter)
            {
                var orgin = msgdata.target;
                msgdata.target = this;
                cb = ()=> msgdata.target = orgin;
            }
            else
            {
                var origin = msg.isStop;
                msg.isStop = true;
                cb = ()=> msg.isStop = origin;
            }
            return cb;
        }
        
        private System.Action Collect_Launcher(Msg msg)
        {
            var msgdata = msg.GetData<MsgData_Collect>();
            var unit = new BaseUnit();
            unit.leaf = this;
            unit.unitObj = unitObj;
            msgdata.state.Set(unit);
            return emptyAction;
        }

        private System.Action Collect_Player(Msg msg)
        {
            bool isPlayer = (abilityData.unitKind & ((int)UnitKind.Player))> 0;
            if(isPlayer)
            {
                msg.isStop = true;
                var msgdata = msg.GetData<MsgData_Collect>();
                var unit = new BaseUnit();
                unit.leaf = this;
                unit.unitObj = unitObj;
                msgdata.state.Set(unit);
            }
            return emptyAction;
        }
    }
}


