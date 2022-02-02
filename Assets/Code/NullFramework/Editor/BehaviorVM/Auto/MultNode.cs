
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using NullFramework.Editor;

namespace NullFramework.Editor
{
    public class MultNode : ByteNode
    {
		[Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Next = 0;
		[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte left = 0;
		[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte right = 0;

        protected override void Init()
        {
            base.Init();
            //Desc = $"乘法";
        }
        public override List<int> GetCode(bool isPositve)
        {
            List<int> list = new List<int>();

			var c1 = GetInputCode("right");
			list.AddRange(c1);
			var c0 = GetInputCode("left");
			list.AddRange(c0);
			list.Add(101);
			if(isPositve)
			{
				var c_next = GetOutputCode("Next");
				list.AddRange(c_next);
			}

            return list;
        }
    }
    
}