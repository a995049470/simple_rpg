using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NullFramework.Runtime
{
    public class Tress<T> : Tress, ILeafDataSetter where T : LeafData
    {
        protected T data;
        public void SetLeafData(LeafData data)
        {
            this.data = data as T;
        }
    }
    //考虑把状态机拆出来 以更加好的方式实现...
    //状态机+普通的父子节点
    public class Tress : Leaf
    {
        //所有状态机子节点
        private Dictionary<int, Leaf> m_FSMLeafs;
        private Stack<int> m_fsmStack;
        
        public Tress() : base()
        {
           m_FSMLeafs = new Dictionary<int, Leaf>();
           m_fsmStack = new Stack<int>();
           m_msgRespondMap = new Dictionary<int, MsgRespond>();
           
        }

        
        //增加信息传导叶节点
        public void AddMsgLeaf(int kind, Leaf leaf)
        {
            if(m_msgRespondMap.TryGetValue(kind, out var result_respont))
            {
                result_respont.AddLeaf(leaf);
            }
            else
            {
                var respond = new MsgRespond();
                respond.AddLeaf(leaf);
                m_msgRespondMap[kind] = respond;
                m_parent?.AddMsgLeaf(kind, this);
            }
        }

        public void RemoveMsgLeaf(int kind, Leaf leaf)
        {
            if(m_msgRespondMap.TryGetValue(kind, out var result_respont))
            {
                result_respont.RemoveLeaf(leaf);
                if(result_respont.IsEmpty())
                {
                    m_msgRespondMap.Remove(kind);
                    m_parent?.RemoveMsgLeaf(kind, leaf);
                }
            }
        }

    
        /// <summary>
        /// 增加叶节点 默认不激活
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="leaf"></param>
        public void AddFSMLeaf(int kind, Leaf leaf, bool isActive = false)
        {
        #if UNITY_EDITOR
            if(m_FSMLeafs.ContainsKey(kind))
            {
                throw new Exception($"原来的 {kind} 节点将被覆盖");
            }
        #endif
            m_FSMLeafs[kind] = leaf;
            leaf.SetLeafKind(kind);
            leaf.SetParent(this);
            if(isActive)
            {
                PushFSMLeaf(kind);
            }
            
        }

        public void RemoveFSMLeaf(int kind)
        {
            if(m_fsmStack.Peek() == kind)
            {
                GetFSMLeaf(m_fsmStack.Pop())?.OnExit();
            }
            m_FSMLeafs.Remove(kind);
        }
        
        public Leaf RemoveFSMLeaf(Leaf leaf)
        {
            if(leaf != null)
            {
                m_FSMLeafs.Remove((int)leaf.Kind);
            }
            return leaf;
        }

        public Leaf GetFSMLeaf(int kind)
        {
            if(m_FSMLeafs.TryGetValue(kind, out var leaf))
            {
                return leaf;
            }
            return null;
        }

        //通用叶结点种类增加叶结点到运行栈
        public void PushFSMLeaf(int kind)
        {
            var leaf = GetFSMLeaf(kind);
            if(m_fsmStack.Count > 0)
            {
                GetFSMLeaf(m_fsmStack.Peek())?.Sleep();           
            }
            leaf.OnEnter();
            m_fsmStack.Push(kind);
        }


        //弹出运行栈顶部的叶结点
        public void PopFSMLeaf(){
            if(m_fsmStack.Count == 0)
            {
                return;
            }
            var leaf = GetFSMLeaf(m_fsmStack.Pop());
            leaf.OnExit();
            if(m_fsmStack.Count > 0)
            {
                //节点苏醒
                GetFSMLeaf(m_fsmStack.Peek())?.Wake();
            }
        }

        /// <summary>
        /// 切换栈顶元素
        /// </summary>
        /// <param name="kind"></param>
        public void FSMSwitch(int kind)
        {
            if(m_fsmStack.Count > 0)
            {
                GetFSMLeaf(m_fsmStack.Pop())?.OnExit();
            }
            var leaf = GetFSMLeaf(kind);
            leaf.OnEnter();
            m_fsmStack.Push(kind);

        }
    }
}
