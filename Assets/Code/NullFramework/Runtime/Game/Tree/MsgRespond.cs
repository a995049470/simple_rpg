using System;
using System.Collections.Generic;
using static NullFramework.Runtime.Leaf;

namespace NullFramework.Runtime
{
    public class MsgRespond
    {
        private MsgListener m_msgAction;
        private List<Leaf> m_nextLeafList;
        
        public MsgRespond()
        {
            m_msgAction = null;
            m_nextLeafList = new List<Leaf>();
        }

        public bool IsEmpty()
        {
            return m_msgAction == null && m_nextLeafList.Count == 0;
        }

        public void AddMsgAction(MsgListener action)
        {
            m_msgAction += action;
        }

        public void RemoveMsgAction(MsgListener action)
        {
            m_msgAction -= action;
        }

        public void AddLeaf(Leaf leaf)
        {
            if(!m_nextLeafList.Contains(leaf))
            {
                m_nextLeafList.Add(leaf);
            }
        }

        public void RemoveLeaf(Leaf leaf)
        {
            m_nextLeafList.Remove(leaf);
        }

        public void Invoke(Msg msg)
        {
            Action cb = null;
            cb = m_msgAction?.Invoke(msg);
            bool isContinue = !msg.isStop;
            if(isContinue)
            {
                var count = m_nextLeafList.Count;
                //信息的取消在运行过程中自行处理
                for (int i = 0; i < count ; i++)
                {
                    var leaf = m_nextLeafList[i];
                    leaf.OnUpdate(msg);
                }
            }
            cb?.Invoke();
        }
        
    }
}

