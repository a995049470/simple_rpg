using System;

namespace NullFramework.Runtime
{
    public abstract class BaseGoapGoalLeaf : Leaf
    {
        //需求的状态合集
        protected StateSet goalStates;
        protected int priority;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListeners(
                (BaseMsgKind.CollectGoapGoal, CollectGoapGoal),
                (BaseMsgKind.CollectWorldState, CollectWorldState)
            );
        }

        protected virtual Action CollectGoapGoal(Msg msg)
        {
            var msgadata = msg.GetData<MsgData_CollectGoapGoal>();
            msgadata.UpdateGoal(priority, this, goalStates);
            return emptyAction;
        }

        protected virtual Action CollectWorldState(Msg msg)
        {
            return emptyAction;
        }
    }
}
