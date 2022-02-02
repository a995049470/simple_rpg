using System;
using System.Collections.Generic;
using NullFramework.Runtime;

namespace NullFramework.Editor
{

    public abstract class SingleEnumNode<T> : ByteNode where T : Enum
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public byte Next = 0;
        public T Value;
        protected override void Init()
        {
            //Desc = $"{typeof(T).Name} 节点";
        }
        public override List<int> GetCode(bool isPositive)
        {
            List<int> list = new List<int>();
            list.AddRange(new int[]{ ByteCodeBehaviorVM.PushValueCode, Value.GetHashCode()});
            if(isPositive)
            {
                var c_next = GetOutputCode("Next");
                list.AddRange(c_next);
            }
            return list;
        }
    }

}
