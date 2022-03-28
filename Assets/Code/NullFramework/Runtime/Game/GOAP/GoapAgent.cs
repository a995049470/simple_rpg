using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{

    //goap的代理类 用以协调goap的实体类合状态机之间
    public class GoapAgent : FSMTress
    {
        //行为列表
        private GoapAction[] goapActions;
        private Stack<GoapAction> executionStack;
        private GoapPlanner planner;
        //数据提供者
        private IGoap goap;
        private GoapAction currentAction;
        private StateSet curruentGoal;

        public GoapAgent(IGoap _goap, GoapAction[] _goapActions) : base()
        {
            goap = _goap;
            goapActions = _goapActions;
            planner = new GoapPlanner(); 
            InitFSM();
        }
       

        private void InitFSM()
        {
            var idleState = new GoapFSMState();
            idleState.AddMsgListener(BaseMsgKind.GoapUpdate, Update_Idle);
            idleState.SetLeafKind(LeafKind.Idle);
            
            var moveState = new GoapFSMState();
            moveState.AddMsgListener(BaseMsgKind.GoapUpdate, Update_Move);
            moveState.SetLeafKind(LeafKind.Move);

            var executeState = new GoapFSMState(); 
            executeState.AddMsgListener(BaseMsgKind.GoapUpdate, Update_Execute);
            executeState.SetLeafKind(LeafKind.Execute);

            this.AddFSMLeaf(idleState);
            this.AddFSMLeaf(moveState);
            this.AddFSMLeaf(executeState);
            this.PushFSMLeaf(LeafKind.Idle);

        }

        
        private void Update_Idle(Msg msg)
        {
            curruentGoal = goap.CreateGoalState();
            var worldState = goap.GetWorldState();
            var queue = planner.CreateExecutionQueue(this, curruentGoal, worldState, goapActions, out bool isSuccess);
            if(isSuccess)
            {
                executionStack = queue;
                FSMSwitch(LeafKind.Execute);
            }
            else
            {
                //通知失败
                goap.PlanFinsih(curruentGoal, PlanResult.Fail);
            }
            
        }

        private void Update_Move(Msg msg)
        {
            
            goap.Move(currentAction);
            //任务中止
            if(!currentAction.IsExcuteableAfterMove(this))
            {
                goap.PlanFinsih(curruentGoal, PlanResult.Cancel);
                FSMSwitch(LeafKind.Idle);
                return;
            }
            
            if(currentAction.CheckInRange(this))
            {
                FSMSwitch(LeafKind.Execute);
            }
        }

        
        /// <summary>
        /// 判断所有行为列表是否完成
        /// </summary>
        /// <returns></returns>
        private bool IsAllActionCompelete()
        {
            return currentAction == null && executionStack.Count == 0;
        }

        /// <summary>
        /// 是否有激活的行为
        /// </summary>
        /// <returns></returns>
        private bool IsHasActiveAction()
        {
            return currentAction != null;
        }

        /// <summary>
        /// 行为完成
        /// </summary>
        private void ActionFinish()
        {
            currentAction = null;
        }

        private void Update_Execute(Msg msg)
        {   
            //所有行为完成
            if(IsAllActionCompelete())
            {
                FSMSwitch(LeafKind.Idle);
                goap.PlanFinsih(curruentGoal, PlanResult.Success);
                return;
            }
            //设置当前行为
            if(!IsHasActiveAction())
            {
                currentAction = executionStack.Pop();
                currentAction.DoReset();
            }

            //距离不够 开始移动
            if(!currentAction.CheckInRange(this))
            {
                FSMSwitch(LeafKind.Move);
                return;
            }

            //行为中止
            if(!currentAction.Excute(this))
            {
                FSMSwitch(LeafKind.Idle);
                goap.PlanFinsih(curruentGoal, PlanResult.Cancel);
            }

            //行为完成
            if(currentAction.IsDone())
            {
                ActionFinish();
            }
            
        }
        
    }
}
