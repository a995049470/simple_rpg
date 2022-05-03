using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    //goap的代理类 用以协调goap的实体类合状态机之间
    //需要一些缓存机制
    public class GoapAgentTress : FSMTress
    {
        //行为列表
        private List<BaseGoapActionLeaf> goapActions;
        private Stack<BaseGoapActionLeaf> executionStack;
        private GoapPlanner planner;
        private BaseGoapActionLeaf currentAction;
        //private StateSet goalStates;
        private StateSet worldStates;
        //ai的控制对象
        private Leaf unit;
        //目标
        private Leaf goalLeaf;

        public GoapAgentTress() : base()
        {
            planner = new GoapPlanner(); 
            goapActions = new List<BaseGoapActionLeaf>();
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners
            (
                (BaseMsgKind.GoapUpdate, GoapUpdate)
            );
        }

        public Action GoapUpdate(Msg msg)
        {
            var msgadata = msg.GetData<MsgData_GoapUpdate>();
            if(IsAllActionCompelete())
            {
                FindActionsToCompleteGoal(msgadata);
            }
            else
            {
                ExecuteActions();
            }
            return emptyAction;
        }

        public override void AddChild(Leaf leaf, bool isActive)
        {
            if(leaf is BaseGoapActionLeaf goapAction)
            {
                goapActions.Add(goapAction);
            }
            base.AddChild(leaf, isActive);
        }
        

        private void FindActionsToCompleteGoal(MsgData_GoapUpdate data)
        {
            // if(goalStates == null) goalStates = new StateSet();
            // else goalStates.Clear();
            
            if(worldStates == null) worldStates = new StateSet();
            else worldStates.Clear();
            
            //收集目标
            var data_collectGoapGoal = new MsgData_CollectGoapGoal();
            data_collectGoapGoal.worldStates = worldStates;
            var msg_collectGoapGoal = new Msg(BaseMsgKind.CollectGoapGoal, data_collectGoapGoal, this, this);
            root.SyncExecuteMsg(msg_collectGoapGoal);
            var goalStates = data_collectGoapGoal.goalStates;
            goalLeaf = data_collectGoapGoal.goalLeaf;
            if(goalLeaf != null)
            {
                //收集世界状态
                var data_collectWorldState = new MsgData_CollectWorldState();
                data_collectWorldState.worldStates = worldStates;
                var msg_collectWorldState = new Msg(BaseMsgKind.CollectWorldState, data_collectWorldState, this, goalLeaf);
                root.SyncExecuteMsg(msg_collectWorldState);

                var queue = planner.CreateExecutionQueue(goalStates, worldStates, goapActions, out bool isSuccess);
                if(isSuccess)
                {
                    executionStack = queue;
                }
                else
                {
                    //计划失败
                }
            }
        }

        private void ExecuteActions()
        {
            if(!IsHasActiveAction())
            {
                currentAction = executionStack.Pop();
                currentAction.DoReset();
            }

            currentAction.Execute(worldStates);
            //行为完成
            if(currentAction.IsDone())
            {
                ActionFinish();
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

    }
}
