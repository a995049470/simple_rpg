using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public class GNode
    {
        public BaseGoapActionLeaf action;
        public GNode last;
        public int cost;
        public StateSet currentState;
        public List<BaseGoapActionLeaf> possibleActions;
        
        public GNode(BaseGoapActionLeaf _action, GNode _last, StateSet _state, List<BaseGoapActionLeaf> _actions)
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
        /// 消耗太高了好像...
        /// </summary>
        /// <returns></returns>
        /// TODO:重写 改写的更加合理一点
        public Stack<BaseGoapActionLeaf> CreateExecutionQueue(StateSet goalStates, StateSet originWorldStates, List<BaseGoapActionLeaf> actions, out bool isSuccess)
        {
            if(goalStates == null || goalStates.Count == 0)
            {
                isSuccess = false;
                return null;
            }
            
            //这个写法有问题...
            var possibleActions = actions;

            //可以改成大小堆
            //用相差的条件数作为排序方式
            var nodeStack = new Stack<GNode>();
            nodeStack.Push(new GNode(null, null, originWorldStates, possibleActions));
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

                    if(!IsIncludeTargetStates(action.Preconditions, node.currentState))
                    {
                        continue;
                    }
                    var currentState = CreateNewWorldState(node.currentState, action.Effects);
                    var isFinishGoal = IsIncludeTargetStates(goalStates, currentState);
                    var remainActions = new List<BaseGoapActionLeaf>();
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

            var actionStack = new Stack<BaseGoapActionLeaf>();
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
        
        List<BaseGoapActionLeaf> GetRemainActions(List<BaseGoapActionLeaf> actions, int removeIndex)
        {
            var remain = new List<BaseGoapActionLeaf>();
            remain.AddRange(actions.GetRange(0, removeIndex));
            remain.AddRange(actions.GetRange(removeIndex + 1, actions.Count - removeIndex - 1));
            return remain;
        }

        /// <summary>
        /// 简单的合并
        /// </summary>
        /// <param name="worldState"></param>
        /// <param name="effcts"></param>
        /// <returns></returns>
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


        // StateSet TryCombineStateSet(StateSet left, StateSet right, out bool isSuccess)
        // {
        //     isSuccess = false;
        //     var keyStates = new Dictionary<int, int>();
        //     int exit_left = 1;//左边存在
        //     int exit_right = 2;//右边存在
        //     int exit_left_right = 3;//两者皆有
        //     foreach (var key in left.Keys)
        //     {
        //         keyStates[key] = exit_left;
        //     }
        //     foreach (var key in right.Keys)
        //     {
        //         if(keyStates.TryGetValue(key, out int value))
        //         {
        //             keyStates[key] = exit_left_right;
        //         }
        //         else
        //         {
        //             keyStates[key] = exit_right;
        //         }
        //     }
        //     var resStates = new StateSet();
        //     foreach (var kvp in keyStates)
        //     {
        //         if(kvp.Value == exit_left)
        //         {
        //             resStates[kvp.Key] = left[kvp.Key];
        //         }
        //         else if(kvp.Value == exit_right)
        //         {
        //             resStates[kvp.Key] = right[kvp.Key];
        //         }
        //         else if(kvp.Value == exit_left_right)
        //         {
        //             var leftValue = left[kvp.Key];
        //             var rightValue = right[kvp.Key];
        //             if(leftValue.TryCombine(rightValue, out var resValue))
        //             {
        //                 resStates[kvp.Key] = resValue;
        //             }
        //             else
        //             {
        //                 isSuccess = false;
        //                 return resStates;
        //             }
        //         }
        //     }
        //     return resStates;
        // }

        /// <summary>
        /// 是否包含所有目标状态
        /// </summary>
        /// <param name="target">动作的前置函数</param>
        /// <param name="states">当前的世界状态</param>
        /// <returns></returns>
        bool IsIncludeTargetStates(StateSet target, StateSet states)
        {
            bool isMeet = true;
            foreach (var condtion in target.Values)
            {
                if(states.TryGetValue(condtion.Key, out State state))
                {
                    if(!state.IsInclude(condtion))
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
