using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NullFramework.Runtime
{
    //实际生效的叶子节点
    public class Leaf : IHandle
    {
        protected static Root m_root { get => Root.Instance; }

        protected Handle<Tress> m_parentHandle;
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
            HandleManager.Instance.Put(this);
        }

        //激活或进入运行栈
        public virtual void OnEnter(Handle<Leaf> lastHandle = default)
        {
            m_wake = true;
            m_active = true;
        }

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
        public virtual void OnExit(Handle<Leaf> nextHandle = default)
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

        //获取句柄
        public Handle<Leaf> GetHandle()
        {
            return new Handle<Leaf>(m_handle);
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

        public virtual void AddMsgListener(int msgKind, Action<Msg> action)
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
                m_parentHandle.Get()?.AddMsgLeafHandle(msgKind, this.GetHandle());
            }
        }

        public virtual void RemoveMsgListener(int msgKind, Action<Msg> action)
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
        //成为信息流动节点
        private void RegisterMsgHandle()
        {
            if(m_isRegisterMsgHandle) return;
            var parent = m_parentHandle.Get();
            if (parent != null)
            {
                m_isRegisterMsgHandle = true;
                foreach (var key in m_msgRespondMap.Keys)
                {
                    parent.AddMsgLeafHandle(key, this.GetHandle());
                }

            }
        }

        
        //TODO:考虑切换父物体的问题
        //考虑切换父物体是 改变handle
        public void SetParent(Handle<Tress> parentHandle)
        {
            if (m_parentHandle == parentHandle)
            {
                return;
            }  
            
            m_parentHandle = parentHandle;
            //只在设置父物体的时候注册一次
            RegisterMsgHandle();
        }
    }
}

