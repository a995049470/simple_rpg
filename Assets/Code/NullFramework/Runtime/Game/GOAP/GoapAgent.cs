using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{

    //goap的代理类 用以协调goap的实体类合状态机之间
    public class GoapAgent : FSMTress
    {
        //行为列表
        private List<GoapActionLeaf> goapActions;
        private Stack<GoapActionLeaf> executionStack;
        private GoapPlanner planner;
        //数据提供者
        private IGoap goap;
        private GoapActionLeaf currentAction;
        private StateSet curruentGoal;

        public GoapAgent() : base()
        {
            planner = new GoapPlanner(); 
            goapActions = new List<GoapActionLeaf>();
        }

        public override void AddChild(Leaf leaf, bool isActive)
        {
            if(leaf is GoapActionLeaf goapAction)
            {
                goapActions.Add(goapAction);
            }
            base.AddChild(leaf, isActive);
        }
        
        

        private void FindActionsToCompleteGoal()
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

        private void ExecuteActions()
        {
             if(!IsHasActiveAction())
            {
                currentAction = executionStack.Pop();
                currentAction.DoReset();
            }

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
