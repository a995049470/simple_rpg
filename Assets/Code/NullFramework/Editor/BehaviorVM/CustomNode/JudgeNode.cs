using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace NullFramework.Editor
{
    public class JudgeNode : ByteNode
    {
		[Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Next_True = 0;
		[Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Next_False = 0;
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Next = 0;
		[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
		public byte Enter = 0;
		protected override void Init()
        {
            base.Init();
            //Desc = $"if节点";
        }
		public override List<int> GetCode(bool isPositve)
        {
            List<int> list = new List<int>();

			if(!isPositve)
			{
				var c_enter = GetInputCode("Enter");
				list.AddRange(c_enter);
			}       

            var code_true = GetOutputCode("Next_True");
            var code_false = GetOutputCode("Next_False");
            
            list.Add(ByteCodeBehaviorVM.JundgeCode);
            list.Add(code_true.Count);
            list.Add(code_false.Count);
            list.AddRange(code_true);
            list.AddRange(code_false);

            if(isPositve)
			{
				var c_next = GetOutputCode("Next");
				list.AddRange(c_next);
			}
            return list;
        }
    }
    
}