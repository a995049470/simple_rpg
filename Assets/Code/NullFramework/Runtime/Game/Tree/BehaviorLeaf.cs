using System;

namespace NullFramework.Runtime
{
    public class BehaviorLeaf : Leaf
    {
        private ByteCodeBehavior m_behavior;
        public bool TryGetResult(out bool isSucess)
        {
            isSucess = false;
            bool isComplte = false;
            if(m_behavior == null)
            {
                isComplte = true;
            }
            else if(m_behavior.IsEnd())
            {
                isSucess = m_behavior.GetExcuteResult();
                isComplte = true;
            }
            return isComplte;
        }
        public void AddBehaviorUpdate(ByteCodeBehavior behavior, Action action)
        {
            m_behavior = behavior;
            AddMsgListener(BaseMsgKind.BehaviorUpdate, _=>action?.Invoke());
        }
    }
}

