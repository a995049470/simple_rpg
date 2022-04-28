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
        //检查动作的先决条件(比如 查找合适的动作目标)
        public abstract bool CheckActionPreconditions(GoapAgent agent);
        //重置变量
        protected abstract void Reset();
        //动作是否完成
        public abstract bool IsDone();
        //判断目标是否在范围内
        public abstract bool CheckInRange(GoapAgent agent);
        //执行动作 可能中途失败
        public abstract bool Excute(GoapAgent agent);
        /// <summary>
        /// 判断移动之后 是否仍旧能执行动作
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public abstract bool IsExcuteableAfterMove(GoapAgent agent);
        
        // public void AddPrecondition(State state)
        // {
        //     preconditions[state.Key] = state;
        // }

        // public void RemovePreconditon(State state)
        // {
        //     preconditions.Remove(state.Key);
        // }

        // public void AddEffect(State state)
        // {
        //     effects[state.Key] = state;
        // }

        // public void  RemoveEffect(State state)
        // {
        //     effects.Remove(state.Key);
        // }

    }
}
