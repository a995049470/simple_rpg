using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public abstract class GoapGoalLeaf : Leaf
    {
        //需求的状态合集
        private StateSet goalStates;
        
        protected abstract Action CollectGoapGoal(Msg msg);

    }
}
