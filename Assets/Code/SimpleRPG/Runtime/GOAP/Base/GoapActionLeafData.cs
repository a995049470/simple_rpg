using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{

    public class GoapActionLeafData<T> : LeafData<T>, IGoapActionLeafData where T : BaseGoapActionLeaf, new()
    {
        [SerializeField]
        private StateData[] preconditions;
        [SerializeField]
        private StateData[] effects;

        public StateSet CreatePreconditions()
        {
            return StateDataListToStateSet(preconditions);
        }

        public StateSet CreateEffects()
        {
            return StateDataListToStateSet(effects);
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
