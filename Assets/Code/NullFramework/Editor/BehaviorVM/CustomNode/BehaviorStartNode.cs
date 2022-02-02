using System.Collections.Generic;

namespace NullFramework.Editor
{
    /// <summary>
    /// 开始节点
    /// </summary>
    public class BehaviorStartNode : ByteNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Next = 0;
        
          protected override void Init()
        {
            base.Init();
            //Desc = $"行为开始节点";
        }

        public override List<int> GetCode(bool isPositive)
        {
            List<int> list = new List<int>();

            if(isPositive)
            {
                var c_next = GetOutputCode("Next");
                list.AddRange(c_next);
            }
            return list;
        }
    }

}
