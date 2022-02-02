using XLua;
using System.Collections;
using System.Collections.Generic;
namespace NullFramework.Runtime
{
    /// <summary>
    /// 条件 目标 状态的基本形容类
    /// </summary>

    //暂时只用int
    public class State
    {
        public int Key;
        public int Value;
        
        public State(int _key, int _value)
        {
            Key = _key;
            Value = _value;
        }

        /// <summary>
        /// 判断该状态能否被消灭
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsDieOut(State state)
        {
            var isPass = Key == state.Key && Value == -state.Value;
            return isPass;
        }

        /// <summary>
        /// 相同的状态叠加
        /// </summary>
        /// <param name="state"></param>
        public void Add(State state)
        {
            
        }
        
        
    }
}
