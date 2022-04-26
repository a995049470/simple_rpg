using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public abstract class Tress<T> : Tress, ILeafDataReciver where T : LeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() { }

        public void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
        }
    }
    
    //考虑把状态机拆出来 以更加好的方式实现...
    //状态机+普通的父子节点
    public class Tress : Leaf
    {
        
        //所有状态机子节点 
        public Tress() : base()
        {
           m_msgRespondMap = new Dictionary<int, MsgRespond>();
           hiddenMsgListenerDic = new Dictionary<int, MsgListener>();
        }
        //潜伏的事件 在该类型事件添加监听时候会一起被加到监听列表
        private Dictionary<int, MsgListener> hiddenMsgListenerDic;
        /// <summary>
        /// 从字节点收集信息
        /// </summary>
        /// <param name="msg"></param>
        protected void CollectInfo(Msg msg)
        {
            OnUpdate(msg);
            
        }

        protected void AddHiddenMsgListener(int msgKind, MsgListener listener)
        {
            if(hiddenMsgListenerDic.TryGetValue(msgKind, out var msgListener))
            {
                msgListener += listener;
            }
            else
            {
                hiddenMsgListenerDic[msgKind] = listener;
            }
        }

        public void AddHiddenMsgListeners(params (int, MsgListener)[] listeners)
        {
            foreach (var listener in listeners)
            {
                AddHiddenMsgListener(listener.Item1, listener.Item2);
            }
        }
        

        //增加信息传导叶节点
        public void AddMsgLeaf(int msgKind, Leaf leaf)
        {
            if(m_msgRespondMap.TryGetValue(msgKind, out var result_respond))
            {
                result_respond.AddLeaf(leaf);
            }
            else
            {
                var respond = new MsgRespond();
                //触发潜伏事件的添加
                if(hiddenMsgListenerDic.TryGetValue(msgKind, out var listener))
                {
                    respond.AddMsgAction(listener);
                    hiddenMsgListenerDic.Remove(msgKind);
                }
                respond.AddLeaf(leaf);
                m_msgRespondMap[msgKind] = respond;
                parent?.AddMsgLeaf(msgKind, this);
            }
        }

        public void RemoveMsgLeaf(int msgKind, Leaf leaf)
        {
            if(m_msgRespondMap.TryGetValue(msgKind, out var result_respond))
            {
                result_respond.RemoveLeaf(leaf);
                if(result_respond.IsEmpty())
                {
                    m_msgRespondMap.Remove(msgKind);
                    parent?.RemoveMsgLeaf(msgKind, leaf);
                }
            }
        }
        

        public virtual void AddChild(Leaf leaf, bool isActive)
        {
            if(isActive)
            {
                leaf.OnEnable();
            }
        }

       
    }
}
