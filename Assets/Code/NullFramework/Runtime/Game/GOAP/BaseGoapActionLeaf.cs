namespace NullFramework.Runtime
{
    /// <summary>
    /// goap行为
    /// </summary>
    public abstract class BaseGoapActionLeaf : Leaf
    {
        //前置条件
        protected StateSet preconditions;
        //完成之后对世界状态的影响
        protected StateSet effects;
        //花费
        int cost;
        //目标是否在范围内
        bool isInRange;
        
        public StateSet Preconditions { get => preconditions;  }
        public StateSet Effects { get => effects; }
        public int Cost { get => cost; }

        public BaseGoapActionLeaf() : base()
        {

        }

        //执行重置
        public void DoReset()
        {
            isInRange = false;
            Reset();
        }
        //重置变量
        protected virtual void Reset() { }
        //动作是否完成
        public virtual bool IsDone() { return true; } 
        //false 表示执行失败
        public virtual bool Execute(StateSet worldStates) { return true; }
        
        /// <summary>
        /// 完成时改变世界的状态
        /// </summary>
        /// <param name="worldStates"></param>
        public void Compelete(StateSet worldStates)
        {

        }
        // public virtual bool CheckActionPreconditions(StateSet worldState) { return true; } 
    }
}
 