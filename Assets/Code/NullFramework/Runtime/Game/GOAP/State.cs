using XLua;
using System.Collections;
using System.Collections.Generic;
namespace NullFramework.Runtime
{
    public enum ValueType
    {
        Empty = 0,
        //Bool = 1,
        Int = 2,
        Float = 3,
        Object = 4,
        //只是个字段 不包含任何值
        Filed = 5,
        
    }
    
    /// <summary>
    /// 条件 目标 状态的基本形容类
    /// </summary>
    //暂时只用int
    public class State
    {
        private int key;
        public int Key { get => key; }
        private int value_int;
        private float vlaue_float;
        private bool value_bool;
        private float value_float;
        private object value_obj;
        private ValueType type;
        private bool isValid;
        public bool IsVaild { get => isValid; }

        public State(int _key = 0, bool _isValid = false)
        {
            if(_isValid)
            {
               Set();
            }
            else
            {
                type = ValueType.Empty;
                isValid = _isValid;
            }
        }

        public State(int _key, int _value)
        {
            key = _key;
            Set(_value);
        }
        public State(int _key, float _value)
        {
            key = _key;
            Set(_value);
        }
        public State(int _key, object _value)
        {
            key = _key;
            Set(_value);
        }

        public void Set(int value)
        {
            value_int = value;
            type = ValueType.Int;
            isValid = true;
        }
        // public void Set(bool value)
        // {
        //     value_bool = value;
        //     type = ValueType.Bool;
        //     isValid = true;
        // }
        public void Set(float value)
        {
            value_float = value;
            type = ValueType.Float;
            isValid = true;
        }
        public void Set(object value)
        {
            value_obj = value;
            type = ValueType.Object;
            isValid = true;
        }
        public void Set()
        {
            type = ValueType.Filed;
            isValid = true;
        }

        public int GetInt()
        {
        #if UNITY_EDITOR
            if(type != ValueType.Int)
            {
                throw new System.Exception("类型不为int");
            }
        #endif
            return value_int;
        }

        public float GetFloat()
        {
        #if UNITY_EDITOR
            if(type != ValueType.Float)
            {
                throw new System.Exception("类型不为float");
            }
        #endif
            return value_float;
        }

        // public bool GetBool()
        // {
        // #if UNITY_EDITOR
        //     if(type != ValueType.Bool)
        //     {
        //         throw new System.Exception("类型不为bool");
        //     }
        // #endif
        //     return value_bool;
        // }

        public T Get<T>() where T : class
        {
            return GetObject() as T;
        }

        public object GetObject()
        {
        #if UNITY_EDITOR
            if(type != ValueType.Object)
            {
                throw new System.Exception("类型不为object");
            }
        #endif
            return value_obj;
        }
        
        public bool IsInclude(State state)
        {
            return true;
        }

        /// <summary>
        /// TODO:相同的状态叠加
        /// </summary>
        /// <param name="state"></param>
        public State Add(State state)
        {
            return default;
        }
        
        /// <summary>
        /// TODO:条件合并 
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
