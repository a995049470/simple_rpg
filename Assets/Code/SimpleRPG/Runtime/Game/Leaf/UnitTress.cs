using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class UnitTress : Tress<UnitTressData>
    {
        private GameObject unitObj;
        private Transform look;
        private int unitKind;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddHiddenMsgListeners(
                (GameMsgKind.Move, Move),
                (GameMsgKind.FollowTarget, FollowTarget),
                (GameMsgKind.Attack, Attack),
                (GameMsgKind.CollectEnemy, CollectEnemy),
                (BaseMsgKind.GoapUpdate, GoapUpdate)
            );
            AddMsgListeners(
                (GameMsgKind.Collect_Launcher, Collect_Launcher),
                (GameMsgKind.Collect_Player, Collect_Player)
            );
        }

        
        public override void OnReciveDataFinish()
        {
            unitObj = leafData.InstantiateUnit();
            look = unitObj.transform.Find("look"); 
            unitKind = ((int)leafData.unitKind);
        }

        public System.Action Move(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Move>();
            System.Action cb = null;
            if((msgData.unitType & unitKind) > 0)
            {
                var origin = msgData.mover;
                msgData.mover = unitObj.transform;
                cb = () => msgData.mover = origin;
            }
            return cb;
        }

        public System.Action FollowTarget(Msg msg)
        {
            var msgData = msg.GetData<MsgData_FollowTarget>();
            var origin = msgData.lookTarget;
            msgData.lookTarget = look;
            return () => msgData.lookTarget = origin;
        }

        public System.Action Attack(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Attack>();
            //var origin_attcker = msgData.attacker;
            if((msgData.attackerFilter & unitKind) > 0)
            {
                var battleUnit = new BattleUnit();
                battleUnit.leaf = this;
                battleUnit.unitObj = this.unitObj;
                battleUnit.unitKind = this.unitKind;
                msgData.attacker = battleUnit;
            }
            return emptyAction;
            // return () => msgData.attacker = origin_attcker;
        }

        private System.Action CollectEnemy(Msg msg)
        {
            var msgData = msg.GetData<MsgData_CollectEnemy>();
            var position = unitObj.transform.position;
            msgData.TryAddEnemy(this, unitObj, unitKind);
            return emptyAction;
        }

        private System.Action GoapUpdate(Msg msg)
        {
            var cb = emptyAction;
            var msgdata = msg.GetData<MsgData_GoapUpdate>();
            var isPassFilter = (unitKind & msgdata.filter) > 0;
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
            bool isPlayer = (unitKind & ((int)UnitKind.Player))> 0;
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


