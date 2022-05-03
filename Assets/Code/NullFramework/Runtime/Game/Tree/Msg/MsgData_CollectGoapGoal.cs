
namespace NullFramework.Runtime
{
    public class MsgData_CollectGoapGoal : MsgData
    {
        //发起者
        public Leaf launcher;
        public Leaf goalLeaf;
        private int maxPriority;
        public StateSet goalStates;
        public StateSet worldStates;
        
        public void UpdateGoal(int _priority, Leaf _goalLeaf, StateSet _goalStates)
        {
            if(maxPriority > _priority) return;
            maxPriority = _priority;
            goalLeaf = _goalLeaf;
            goalStates = _goalStates;

        }
    }
}

