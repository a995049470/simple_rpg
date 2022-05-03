using System.Collections.Generic;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    
    public class SimpleAttackLeaf : LuaLeaf<SimpleAttackLeafData>
    {
        //直接public给Lua调用

        public float attackRadius;
        public int attackNum;
        public int attackFilter;
        
        

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            SetLuaCodeByFile(leafData.luaFile);
            this.attackRadius = leafData.attackRadius;
            this.attackNum = leafData.attackNum;
            this.attackFilter = ((int)leafData.attackFilter);
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners
            (
                (GameMsgKind.Attack, Attack)
            );
        }
        //如果有多个战斗信号输入?
        private System.Action Attack(Msg msg)
        {
            var msgData = msg.GetData<MsgData_Attack>();
            //TODO:考虑某些情况下不接受战斗数据的输入

            //var origin = msgData.hiters;
            //数据未被初始化
            if(msgData.attackState == RunState.None)
            {
                //收集敌人
                msgData.hiters = new List<BattleUnit>();
                var data_collectEnemy = new MsgData_CollectEnemy();
                data_collectEnemy.hiters = msgData.hiters;
                data_collectEnemy.radius = this.attackRadius;
                data_collectEnemy.SetAttackNum(this.attackNum);
                data_collectEnemy.center = msgData.attacker.unitObj.transform.position;
                data_collectEnemy.attackFilter = attackFilter;
                var msg_collectEnemy = new Msg(GameMsgKind.CollectEnemy, data_collectEnemy);
                root.SyncExecuteMsg(msg_collectEnemy);
                msgData.attackState = RunState.Runing;
            }
            int state = ExecuteLuaScript(msgData);
            msgData.attackState = state;

            //下一帧继续攻击命令的执行
            if(state == RunState.Runing)
            {
                var nextFrameMsg = new Msg(GameMsgKind.Attack, msgData, this, this);
                AddNextFrameMsg(nextFrameMsg);
            }
            //战斗结束
            else{
                
            }
            //开始攻击流程
            return null;
            //return ()=> msgData.hiters = origin;        
        }
    }
}

