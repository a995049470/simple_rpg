using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class UnitTress : Tress<UnitTressData>
    {
        private GameObject unit;
        private Transform look;
        private int unitKind;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddHiddenMsgListeners(
                (GameMsgKind.Move, Move),
                (GameMsgKind.FollowTarget, FollowTarget),
                (GameMsgKind.Attack, Attack),
                (GameMsgKind.CollectEnemy, CollectEnemy)
            );
        }

        
        public override void OnReciveDataFinish()
        {
            unit = leafData.InstantiatePlayer();
            look = unit.transform.Find("look"); 
            unitKind = ((int)leafData.unitKind);
        }

        public System.Action Move(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Move>();
            System.Action cb = null;
            if((msgData.unitType & unitKind) > 0)
            {
                var origin = msgData.mover;
                msgData.mover = unit.transform;
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
            var origin_attcker = msgData.attacker;
            var battleUnit = new BattleUnit();
            battleUnit.leaf = this;
            battleUnit.unitObj = this.unit;
            battleUnit.unitKind = this.unitKind;
            msgData.attacker = battleUnit;
            return () => msgData.attacker = origin_attcker;
        }

        private System.Action CollectEnemy(Msg msg)
        {
            var msgData = msg.GetData<MsgData_CollectEnemy>();
            
            var origin = msgData.isCurrentEnemyInRange;
            var position = unit.transform.position;
            msgData.TryAddEnemy(this, unit, unitKind);
            return () => msgData.isCurrentEnemyInRange = origin;
        }
    }
}


