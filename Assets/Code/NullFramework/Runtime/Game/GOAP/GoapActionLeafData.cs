using UnityEngine;
namespace NullFramework.Runtime
{

    public class GoapActionLeafData<T> : LeafData<T> where T : GoapActionLeaf, new()
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
