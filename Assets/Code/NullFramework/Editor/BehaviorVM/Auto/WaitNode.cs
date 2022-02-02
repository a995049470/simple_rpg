
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using NullFramework.Editor;

namespace NullFramework.Editor
{
    public class WaitNode : ByteNode
    {
		[Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Next = 0;
		[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte time = 0;

        protected override void Init()
        {
            base.Init();
            //Desc = $"等待";
        }
        public override List<int> GetCode(bool isPositve)
        {
            List<int> list = new List<int>();

			var c0 = GetInputCode("time");
			list.AddRange(c0);
			list.Add(103);
			if(isPositve)
			{
				var c_next = GetOutputCode("Next");
				list.AddRange(c_next);
			}

            return list;
        }
    }
    
}