using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class GoapGoalLeaf<T> : BaseGoapGoalLeaf where T : LeafData, IGoapGoalLeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() {}

        public virtual void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
            goalStates = (data as IGoapGoalLeafData).CreateGoalStates();
        }
    }
}
