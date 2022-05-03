using System;

namespace NullFramework.Runtime
{
    public abstract class BaseGoapGoalLeaf : Leaf
    {
        //需求的状态合集
        protected StateSet goalStates;
        protected virtual Action CollectGoapGoal(Msg msg)
        {
            return emptyAction;
        }
    }
}
