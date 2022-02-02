using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NullFramework.Runtime
{
    //状态机+普通的父子节点
    public class Tress : Leaf
    {
        //所有子节点
        private Dictionary<int, List<Handle<Leaf>>> m_childLeafHandles;
        private Stack<Handle<Leaf>> m_fsmStack;
        
    
        public Tress() : base()
        {
           m_childLeafHandles = new Dictionary<int, List<Handle<Leaf>>>();
        }

        //增加信息传导叶节点
        public void AddMsgLeafHandle(int kind, Handle<Leaf> leafHandle)
        {
            if(m_msgRespondMap.TryGetValue(kind, out var result_respont))
            {
                result_respont.AddLeafHandle(leafHandle);
            }
            else
            {
                var respond = new MsgRespond();
                respond.AddLeafHandle(leafHandle);
                m_msgRespondMap[kind] = respond;
                m_parentHandle.Get()?.AddMsgLeafHandle(kind, this.GetHandle());
            }
        }

   
        /// <summary>
        /// 增加叶节点 默认不激活
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="leaf"></param>
        public void AddLeaf(int kind, Leaf leaf, bool isActive = false)
        {
            var leafHandle = leaf.GetHandle();
            if(m_childLeafHandles.TryGetValue(kind, out var list))
            {
                list.Add(leafHandle);
            }
            else
            {
                var leafHandleList = new List<Handle<Leaf>>();
                leafHandleList.Add(leafHandle);
                m_childLeafHandles[kind] = leafHandleList;
            }
            leaf.SetLeafKind(kind);
            if(isActive)
            {
                leaf.OnEnter();
            }
            
            leaf.SetParent(this.GetHandle().Convert<Tress>());
        }
        
        /// <summary>
        /// 移除叶节点 句柄也会失效
        /// </summary>
        /// <param name="leafHandle"></param>
        /// <returns></returns>
        public Leaf RemoveLeaf(Handle<Leaf> leafHandle)
        {
            var leaf = leafHandle.Get();
            if(leaf != null)
            {
                m_childLeafHandles[leaf.Kind].Remove(leafHandle);
                leafHandle.Free();
            }
            return leaf;
        }

        public Handle<Leaf> GetSingleLeafHandle(int kind)
        {
        #if UNITY_EDITOR
             Debug.Assert(m_childLeafHandles.ContainsKey(kind) && m_childLeafHandles[kind].Count > 0, "无效种类");
        #endif
            return m_childLeafHandles[kind][0];
        }

        public List<Handle<Leaf>> GetLeafHandles(int kind)
        {
            return m_childLeafHandles[kind];
        }
        

        //通用叶结点种类增加叶结点到运行栈
        public void PushLeaf(int kind)
        {
            var leafHandle = GetSingleLeafHandle(kind);
            if(m_fsmStack.Count > 0)
            {
                //节点休眠
                m_fsmStack.Peek().Get()?.Sleep();                
            }
            leafHandle.Get()?.OnEnter();
            m_fsmStack.Push(leafHandle);
        }


        //弹出运行栈顶部的叶结点
        public void PopLeaf(){
            if(m_fsmStack.Count == 0)
            {
                return;
            }
            var leafHandle = m_fsmStack.Pop();
            leafHandle.Get()?.OnExit();
            if(m_fsmStack.Count > 0)
            {
                //节点苏醒
                m_fsmStack.Peek().Get()?.Wake();
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
                m_fsmStack.Pop().Get()?.OnExit();
            }
            var leafHandle = GetSingleLeafHandle(kind);
            leafHandle.Get()?.OnEnter();
            m_fsmStack.Push(leafHandle);

        }
    }
}
