using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class EnemyTress : Tress<EnemyTressData>
    {
        private GameObject enemy;
        public override void OnReciveDataFinish()
        {
            enemy = leafData.InstantiateEnemy();
        }

        protected override void InitListeners()
        {
            AddMsgListeners
            (
                (GameMsgKind.CollectEnemy, CollectEnemy)
            );
        }
        private System.Action CollectEnemy(Msg msg)
        {
            var msgData = msg.GetData<MsgData_CollectEnemy>();
            var origin = msgData.isCurrentEnemyInRange;
            var position = enemy.transform.position;
            msgData.TryAddEnemy(this, enemy);
            return () => msgData.isCurrentEnemyInRange = origin;
        }
        
    }
}


