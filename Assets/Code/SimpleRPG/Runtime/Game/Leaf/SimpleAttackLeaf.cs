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
        public float coolDownTime;
        private float lastAttackTime;
        private MsgData_Attack cacheData;
        

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            SetLuaCodeByFile(leafData.luaFile);
            this.attackRadius = leafData.attackRadius;
            this.attackNum = leafData.attackNum;
            this.attackFilter = ((int)leafData.attackFilter);
            this.coolDownTime = leafData.coolDownTime;
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners
            (
                (GameMsgKind.Attack, Attack),
                (GameMsgKind.Collect_AttackCoolDown, Collect_AttackCoolDown),
                (GameMsgKind.Collect_InAttackRange, Collect_InAttackRange)
            );
        }
        
        private bool IsCooldownFinish()
        {
            return root.CurrentTime - lastAttackTime > coolDownTime;
        }

        private bool IsAttackRuning()
        {
            bool isRuning = false;
            if(cacheData != null)
            {
                isRuning = cacheData.attackState == RunState.Runing;
            }
            return isRuning;
        }

        private bool IsCahcheData(MsgData msgData)
        {
            return msgData == cacheData;
        }

        private System.Action Collect_AttackCoolDown(Msg msg)
        {
            var msgdata = msg.GetData<MsgData_Collect>();
            if(IsCooldownFinish())
            {
                msgdata.state.Set();
            }
            return emptyAction;
        }

        private System.Action Collect_InAttackRange(Msg msg)
        {
            var msgdata = msg.GetData<MsgData_Collect>();
            var launcher = msgdata.worldStates[((int)GameStateType.Launcher)].Get<BaseUnit>();
            var attackTarget = msgdata.worldStates[((int)GameStateType.AttackTarget)].Get<BaseUnit>();
            var pos_launcher = launcher.unitObj.transform.position;
            var pos_attackTarget = attackTarget.unitObj.transform.position;
            var dis = UnityEngine.Vector3.Distance(pos_attackTarget, pos_launcher);
            if(dis < attackRadius)
            {
                msgdata.state.Set();
            }
            return emptyAction;
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
                //冷却完成&&并且没有战斗在执行
                if(IsCooldownFinish() && !IsAttackRuning())
                {
                    cacheData = msgData;
                    lastAttackTime = root.CurrentTime;
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
                else{
                    msgData.attackState = RunState.Fail;
                }
            }           
            
            if(msgData.attackState == RunState.Runing && IsCahcheData(msgData))
            {
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
                    cacheData = null;
                }
            }

            return null;
      
        }
    }
}

