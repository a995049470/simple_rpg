using System.Collections.Generic;

namespace NullFramework.Runtime
{
    /// <summary>
    /// goap行为
    /// </summary>
    public abstract class GoapActionLeaf : Leaf
    {
        //前置条件
        StateSet preconditions;
        //完成之后对世界状态的影响
        StateSet effects;
        //动作目标
        Leaf target;
        //花费
        int cost;
        //目标是否在范围内
        bool isInRange;
        
        public StateSet Preconditions { get => preconditions;  }
        public StateSet Effects { get => effects; }
        public int Cost { get => cost; }

        public GoapActionLeaf() : base()
        {

        }

        //执行重置
        public void DoReset()
        {
            target = null;
            isInRange = false;
            Reset();
        }
        //重置变量
        protected abstract void Reset();
        //动作是否完成
        public abstract bool IsDone();
        
        public abstract bool Execute(StateSet worldState);
    }
}
