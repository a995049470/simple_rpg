using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class GoapGoalLeafData<T> : LeafData<T>, IGoapGoalLeafData where T : BaseGoapGoalLeaf, new()
    {
        [SerializeField]
        private StateData[] goalStates;

        public StateSet CreateGoalStates()
        {
            return StateDataListToStateSet(goalStates);
        }
        protected StateSet StateDataListToStateSet(StateData[] stateDatas)
        {
            var set = new StateSet();
            foreach (var data in stateDatas)
            {
                var state = data.Create();
                set[state.Key] = state;
            }
            return set;
        }
    }
}
