using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public enum GoalKind
    {
        None = 0,
    }

    public abstract class GoapLeaf : Leaf
    {
        //目标种类
        private GoalKind goal;
        //需求的状态合集
        private StateSet stateSet;


    }
}
