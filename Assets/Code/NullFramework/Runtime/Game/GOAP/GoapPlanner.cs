using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public class GNode
    {
        public GoapAction action;
        public GNode last;
        public int cost;
        public StateSet currentState;
        public List<GoapAction> possibleActions;
        
        public GNode(GoapAction _action, GNode _last, StateSet _state, List<GoapAction> _actions)
        {
            action = _action;
            if(action != null) cost += action.Cost;
            last = _last;
            currentState = _state;
            possibleActions = _actions;
        }
    }

    public class GoapPlanner 
    {
        private const int maxLoopCount = 2048;
        /// <summary>
        /// 获取行为列表 尝试扔到多线程里跑
        /// </summary>
        /// <returns></returns>
        public Stack<GoapAction> CreateExecutionQueue(GoapAgent agent, StateSet goal, StateSet worldState, GoapAction[] actions, out bool isSuccess)
        {
            //挑选所有能够执行的行为
            var possibleActions = new List<GoapAction>();
            foreach (var action in actions)
            {
                if(action.CheckActionPreconditions(agent))
                {
                    possibleActions.Add(action);
                }
            }

            var nodeStack = new Stack<GNode>();
            nodeStack.Push(new GNode(null, null, worldState, possibleActions));
            GNode minCostPlanNode = null;

            for (int i = 0; i < maxLoopCount; i++)
            {
                if(nodeStack.Count == 0)
                {
                    break;
                }
                var node = nodeStack.Pop();
                if(minCostPlanNode != null && node.cost >= minCostPlanNode.cost)
                {
                    continue;
                }
                
                for (int j = 0; j < node.possibleActions.Count; j++)
                {
                    var action = node.possibleActions[i];
                    if(minCostPlanNode != null && (node.cost + action.Cost) >= minCostPlanNode.cost)
                    {
                        continue;
                    }
                    if(!IsIncludeTargetStates(action.Preconditions, worldState))
                    {
                        continue;
                    }
                    var currentState = CreateNewWorldState(worldState, action.Effects);
                    var isFinishGoal = IsIncludeTargetStates(goal, currentState);
                    var remainActions = new List<GoapAction>();
                    if(!isFinishGoal)
                    {
                        //假设每个行为在一个计划中只会执行一次
                        remainActions = GetRemainActions(node.possibleActions, i);
                    }
                    var currentNode = new GNode(action, node, currentState, remainActions);
                    if(isFinishGoal)
                    {
                        minCostPlanNode = currentNode;
                    }
                    else
                    {
                        nodeStack.Push(currentNode);
                    }
                }

            }

            var actionStack = new Stack<GoapAction>();
            if(minCostPlanNode != null)
            {
                isSuccess = true;
                var node = minCostPlanNode;
                while (node?.action != null)
                {
                    actionStack.Push(node.action);
                    node = node.last;
                }
            }
            else
            {
                isSuccess = false;
            }
            return actionStack;
        }   
        
        List<GoapAction> GetRemainActions(List<GoapAction> actions, int removeIndex)
        {
            var remain = new List<GoapAction>();
            remain.AddRange(actions.GetRange(0, removeIndex));
            remain.AddRange(actions.GetRange(removeIndex + 1, actions.Count - removeIndex - 1));
            return remain;
        }

        StateSet CreateNewWorldState(StateSet worldState, StateSet effcts)
        {
            var res = new StateSet();
            foreach (var kvp in worldState)
            {
                res[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in effcts)
            {
                res[kvp.Key] = kvp.Value;
            }
            return res;
        }

        /// <summary>
        /// 尝试合并两个集合
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        StateSet TryCombineStateSet(StateSet left, StateSet right, out bool isSuccess)
        {
            isSuccess = false;
            var keyStates = new Dictionary<int, int>();
            int exit_left = 1;//左边存在
            int exit_right = 2;//右边存在
            int exit_left_right = 3;//两者皆有
            foreach (var key in left.Keys)
            {
                keyStates[key] = exit_left;
            }
            foreach (var key in right.Keys)
            {
                if(keyStates.TryGetValue(key, out int value))
                {
                    keyStates[key] = exit_left_right;
                }
                else
                {
                    keyStates[key] = exit_right;
                }
            }
            var res = new StateSet();
            foreach (var kvp in keyStates)
            {
                if(kvp.Value == exit_left)
                {
                    res[kvp.Key] = left[kvp.Key];
                }
                else if(kvp.Value == exit_right)
                {
                    res[kvp.Key] = right[kvp.Key];
                }
                else if(kvp.Value == exit_left_right)
                {
                    var leftValue = left[kvp.Key];
                    var rightValue = right[kvp.Key];
                    if(leftValue != rightValue)
                    {
                        isSuccess = false;
                        return res;
                    }
                    res[kvp.Key] = leftValue;
                }
            }
            return res;
        }

        /// <summary>
        /// 是否包含所有目标状态
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        bool IsIncludeTargetStates(StateSet target, StateSet states)
        {
            bool isMeet = true;
            foreach (var condtion in target.Values)
            {
                if(states.TryGetValue(condtion.Key, out State state))
                {
                    if(condtion.Value != state.Value)
                    {
                        isMeet = false;
                        break;
                    }
                }
                else
                {
                    isMeet = false;
                    break;
                }
            }

            return isMeet;
        }    
    }
}
