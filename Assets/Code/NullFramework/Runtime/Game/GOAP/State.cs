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
        private enum ValueType
        {
            Empty       = 0,
            Bool        = 1,
            Int         = 2,
            Float       = 3,
            Leaf        = 4
        }

        private int key;
        public int Key { get => key; }
        private int value_int;
        private float vlaue_float;
        private bool value_bool;
        private float value_float;
        private Leaf value_leaf;
        private ValueType type;

        public State()
        {
        }

        public State(int _key, int _value)
        {
            key = _key;
        }

        public void SetInt(int value)
        {
            value_int = value;
            type = ValueType.Int;
        }

        public void SetBool(bool value)
        {
            value_bool = value;
            type = ValueType.Bool;
        }

        public bool IsInclude(State state)
        {
            return true;
        }

        /// <summary>
        /// 相同的状态叠加
        /// </summary>
        /// <param name="state"></param>
        public State Add(State state)
        {
            return default;
        }
        
        /// <summary>
        /// 条件合并
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool TryCombine(State target, out State state)
        { 
            state = new State();
            return true;
        }
        
    }
}
