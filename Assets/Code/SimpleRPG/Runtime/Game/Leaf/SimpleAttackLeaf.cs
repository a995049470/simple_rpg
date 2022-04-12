using System.Collections.Generic;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class SimpleAttackLeaf : LuaLeaf<SimpleAttackLeafData>
    {
        //直接public给Lua调用

        public float attackRadius;
        public int attackNum;

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            SetLuaCodeByFile(leafData.luaFile);
            this.attackRadius = leafData.attackRadius;
            this.attackNum = leafData.attackNum;
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
            //收集敌人
            var origin = msgData.hiters;
            msgData.hiters = new List<BattleUnit>();
            var data_collectEnemy = new MsgData_CollectEnemy();
            data_collectEnemy.hiters = msgData.hiters;
            data_collectEnemy.radius = this.attackRadius;
            data_collectEnemy.SetAttackNum(this.attackNum);
            data_collectEnemy.center = msgData.attacker.unitObj.transform.position;
            var msg_collectEnemy = new Msg(GameMsgKind.CollectEnemy, data_collectEnemy);
            root.SyncExecuteMsg(msg_collectEnemy);
            ExecuteLuaScript(msgData);
            //开始攻击流程
            return ()=> msgData.hiters = origin;        
        }
    }
}

