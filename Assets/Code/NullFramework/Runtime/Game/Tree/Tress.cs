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
        }

        //增加信息传导叶节点
        public void AddMsgLeaf(int kind, Leaf leaf)
        {
            if(m_msgRespondMap.TryGetValue(kind, out var result_respond))
            {
                result_respond.AddLeaf(leaf);
            }
            else
            {
                var respond = new MsgRespond();
                respond.AddLeaf(leaf);
                m_msgRespondMap[kind] = respond;
                parent?.AddMsgLeaf(kind, this);
            }
        }

        public void RemoveMsgLeaf(int kind, Leaf leaf)
        {
            if(m_msgRespondMap.TryGetValue(kind, out var result_respond))
            {
                result_respond.RemoveLeaf(leaf);
                if(result_respond.IsEmpty())
                {
                    m_msgRespondMap.Remove(kind);
                    parent?.RemoveMsgLeaf(kind, leaf);
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
