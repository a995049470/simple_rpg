using System;
using System.Collections.Generic;

namespace NullFramework.Runtime
{
    public abstract class Leaf<T> : Leaf, ILeafDataReciver where T : LeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() {}

        public virtual void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
        }
    }
    //实际生效的叶子节点
    public abstract class Leaf 
    {
        protected static Root m_root { get => Root.Instance; }

        protected Tress m_parent;
        protected Dictionary<int, MsgRespond> m_msgRespondMap;
        protected int m_handle;
        //叶节点种类
        private int m_kind;
        private bool m_active = false;
        private bool m_wake = false;

        public int Kind { get => m_kind; }
        private bool m_isRegisterMsgHandle = false;
        
        //同一种kind 不同序号
        //private int m_index;
        public Leaf()
        {
            m_msgRespondMap = new Dictionary<int, MsgRespond>();
        }


        public virtual void LoadData(LeafData data)
        {
            if(this is ILeafDataReciver setter)
            {
                setter.SetLeafData(data);
                setter.OnReciveDataFinish();
            }
        }

        //激活或进入运行栈
        public virtual void OnEnter(Leaf lastLeaf = null)
        {
            m_wake = true;
            m_active = true;
        }

        protected virtual void InitListeners() {}

        /// <summary>
        /// 在激活的状态下是否存在回应
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool OnUpdate(Msg msg)
        {
            bool isHasRespond = true;
            //失活态不除发OnUpdate
            if(!m_active || !m_wake)
            {
                return isHasRespond;
            }

            if(m_msgRespondMap.TryGetValue(msg.Kind, out var respond))
            {
                respond.Invoke(msg);
                isHasRespond = !respond.IsEmpty();
            }
            else
            {
                isHasRespond = false;
            }
            return isHasRespond;
        }

        //失去活力或退出运行栈时
        public virtual void OnExit(Leaf nextHandle = default)
        {   
            m_wake = false;
            m_active = false;
        }
        
        //状态机节点休眠 为下弹式状态机设计
        public void Sleep()
        {
            m_wake = false;
        }

        //状态机节点苏醒 为下弹式状态机设计
        public void Wake()
        {
            m_wake = true;
        }

        

        public void SetLeafKind(int kind)
        {
            m_kind = kind;
        }

        

        public void SetHandle(int handle)
        {
            m_handle = handle;
        }

        public int GetHandle_I32()
        {
            return m_handle;
        }
        //被释放时触发
        public virtual void OnFree()
        {

        }

        public void AddMsgListener(int msgKind, Action<Msg> action)
        {
            if (m_msgRespondMap.TryGetValue(msgKind, out var result_respond))
            {
                result_respond.AddMsgAction(action);
            }
            else
            {
                var msgRespond = new MsgRespond();
                msgRespond.AddMsgAction(action);
                m_msgRespondMap[msgKind] = msgRespond;
                m_parent?.AddMsgLeaf(msgKind, this);
            }
        }

        public void RemoveMsgListener(int msgKind, Action<Msg> action)
        {
            if (m_msgRespondMap.TryGetValue(msgKind, out var respond))
            {
                respond.RemoveMsgAction(action);
                if (respond.IsEmpty())
                {
                    m_msgRespondMap.Remove(msgKind);  
                }
            }
        }

        public void ClearMsgListeners()
        {
            foreach (var key in m_msgRespondMap.Keys)
            {
                m_parent.RemoveMsgLeaf(key, this);
            }
            m_msgRespondMap.Clear();
        }

        //成为信息流动节点
        private void RegisterMsgForParent()
        {
            if(m_isRegisterMsgHandle) return;
            var parent = m_parent;
            if (parent != null)
            {
                m_isRegisterMsgHandle = true;
                foreach (var key in m_msgRespondMap.Keys)
                {
                    parent.AddMsgLeaf(key, this);
                }

            }
        }

        //TODO:考虑切换父物体的问题
        //考虑切换父物体是 改变handle
        public void SetParent(Tress parent)
        {
            if (m_parent == parent)
            {
                return;
            }  
            if(m_parent != null)
            {
                ClearMsgListeners();
            }
            m_parent = parent;
            InitListeners();

            //传输数据
            if(this is ILeafReciver reciver)
            {
                reciver.ReciveLeaf(parent);
            }
        }

        //释放
        public virtual void Free()
        {

        }
    }
}

