using XLua;
using System.Collections;
using System.Collections.Generic;
namespace NullFramework.Runtime
{
    /// <summary>
    /// 条件 目标 状态的基本形容类
    /// </summary>

    //暂时只用int
    public struct State
    {
        private int key;
        public int Key { get => key; }
        private int value;
        
        public State(int _key, int _value)
        {
            key = _key;
            value = _value;
        }

        public bool IsInclude(State state)
        {
            return value == state.value;
        }

        /// <summary>
        /// 判断该状态能否被消灭
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsDieOut(State state)
        {
            var isPass = Key == state.Key && value == -state.value;
            return isPass;
        }

        /// <summary>
        /// 相同的状态叠加
        /// </summary>
        /// <param name="state"></param>
        public State Add(State state)
        {
            return new State(state.key, state.value);
        }
        
        /// <summary>
        /// 条件合并
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool TryCombine(State target, out State state)
        {
            state = new State(target.Key, target.value);
            bool isAllowCombine = state.value == target.value;
            return isAllowCombine;
        }
        
    }
}
