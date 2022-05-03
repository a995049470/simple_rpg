using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class GoapGoalLeafData<T> : LeafData<T>, IGoapGoalLeafData where T : BaseGoapGoalLeaf, new()
    {

        [SerializeField]
        private int priority;
        [SerializeField]
        private StateData[] goalStates;
        
        public StateSet CreateGoalStates()
        {
            return StateDataListToStateSet(goalStates);
        }

        public int GetPriority()
        {
            return priority;
        }

        protected StateSet StateDataListToStateSet(StateData[] stateDatas)
        {
            var states = new StateSet();
            foreach (var data in stateDatas)
            {
                var state = data.Create();
                states[state.Key] = state;
            }
            return states;
        }
    }
}
