using System;

namespace NullFramework.Runtime
{
    public class BehaviorLeaf : Leaf
    {
        private ByteCodeBehavior m_behavior;
        
        public void AddBehaviorUpdate(ByteCodeBehavior behavior, Action action)
        {
            m_behavior = behavior;
            AddMsgListener(BaseMsgKind.BehaviorUpdate, _=>action?.Invoke());
        }
    }
}

