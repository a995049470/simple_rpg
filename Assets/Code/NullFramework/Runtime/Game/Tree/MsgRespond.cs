using System;
using System.Collections.Generic;

namespace NullFramework.Runtime
{
    public class MsgRespond
    {
        private Action<Msg> m_msgAction;
        private List<Handle<Leaf>> m_nextLeafHandleList;
        
        public MsgRespond()
        {
            m_msgAction = null;
            m_nextLeafHandleList = new List<Handle<Leaf>>();
        }

        public bool IsEmpty()
        {
            return m_msgAction == null && m_nextLeafHandleList.Count == 0;
        }

        public void AddMsgAction(Action<Msg> action)
        {
            m_msgAction += action;
        }

        public void RemoveMsgAction(Action<Msg> action)
        {
            m_msgAction -= action;
        }

        public void AddLeafHandle(Handle<Leaf> leafHandle)
        {
            if(!m_nextLeafHandleList.Contains(leafHandle))
            {
                m_nextLeafHandleList.Add(leafHandle);
            }
        }

        public void RemoveLeafHandle(Handle<Leaf> leafHandle)
        {
            m_nextLeafHandleList.Remove(leafHandle);
        }

        public void Invoke(Msg msg)
        {
            m_msgAction?.Invoke(msg);
            var count = m_nextLeafHandleList.Count;
            //信息的取消在运行过程中自行处理
            for (int i = count - 1; i >= 0 ; i--)
            {
                var leaf = m_nextLeafHandleList[i].Get();
                if(leaf == null)
                {
                    //清除无效节点
                    m_nextLeafHandleList.RemoveAt(i);
                }
                else
                {
                    if(!leaf.OnUpdate(msg))
                    {
                        m_nextLeafHandleList.RemoveAt(i);
                    }
                }
            }
        }
    }
}

