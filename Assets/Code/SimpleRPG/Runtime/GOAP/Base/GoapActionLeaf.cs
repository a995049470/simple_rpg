using System.Collections.Generic;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class GoapActionLeaf<T> : BaseGoapActionLeaf where T : LeafData, IGoapActionLeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() {}

        public virtual void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
            var actionData = data as IGoapActionLeafData;
            preconditions = actionData.CreatePreconditions();
            effects = actionData.CreateEffects();
        }
    }
}
