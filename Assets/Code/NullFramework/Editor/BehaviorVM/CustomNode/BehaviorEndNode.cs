using System.Collections.Generic;

namespace NullFramework.Editor
{
    /// <summary>
    /// 开始节点
    /// </summary>
    public class BehaviorEndNode : ByteNode
    {
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Enter = 0;
        
        protected override void Init()
        {
            base.Init();
            //Desc = $"行为开始节点";
        }

        public override List<int> GetCode(bool isPositive)
        {
            List<int> list = new List<int>();

            if(!isPositive)
            {
                var c_next = GetInputCode("Enter");
                list.AddRange(c_next);
            }
            return list;
        }
    }

}
