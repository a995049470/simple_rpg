using System.Collections.Generic;
using System;

namespace NullFramework.Runtime
{
    public class FSMTress : Tress
    {
        private Dictionary<int, Leaf> fsmLeafs;
        private Stack<int> fsmStack;
        
        public FSMTress() : base()
        {
            fsmLeafs = new Dictionary<int, Leaf>();
            fsmStack = new Stack<int>();
        }

        /// <summary>
        /// 增加叶节点 默认不激活
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="leaf"></param>
        public void AddFSMLeaf(Leaf leaf, bool isActive = false)
        {
            int kind = leaf.Kind;
        #if UNITY_EDITOR
            if(fsmLeafs.ContainsKey(kind))
            {
                throw new Exception($"原来的 {kind} 节点将被覆盖");
            }
        #endif
            fsmLeafs[kind] = leaf;
            leaf.SetParent(this);
            if(isActive)
            {
                PushFSMLeaf(kind);
            }
            
        }


        public void RemoveFSMLeaf(int kind)
        {
            if(fsmStack.Peek() == kind)
            {
                GetFSMLeaf(fsmStack.Pop())?.OnDisable();
            }
            fsmLeafs.Remove(kind);
        }
        
        public Leaf RemoveFSMLeaf(Leaf leaf)
        {
            if(leaf != null)
            {
                fsmLeafs.Remove((int)leaf.Kind);
            }
            return leaf;
        }

        public Leaf GetFSMLeaf(int kind)
        {
            if(fsmLeafs.TryGetValue(kind, out var leaf))
            {
                return leaf;
            }
            return null;
        }

        //通用叶结点种类增加叶结点到运行栈
        public void PushFSMLeaf(int kind)
        {
            var leaf = GetFSMLeaf(kind);
            if(fsmStack.Count > 0)
            {
                GetFSMLeaf(fsmStack.Peek())?.Sleep();           
            }
            leaf.OnEnable();
            fsmStack.Push(kind);
        }


        //弹出运行栈顶部的叶结点
        public void PopFSMLeaf()
        {
            if(fsmStack.Count == 0)
            {
                return;
            }
            var leaf = GetFSMLeaf(fsmStack.Pop());
            leaf.OnDisable();
            if(fsmStack.Count > 0)
            {
                //节点苏醒
                GetFSMLeaf(fsmStack.Peek())?.Wake();
            }
        }

        //改变子节点的激活状态
        public void SetLeafActiveState(int kind, bool _active)
        {
            fsmLeafs[kind].SetActive(_active);
        }

        /// <summary>
        /// 切换栈顶元素
        /// </summary>
        /// <param name="kind"></param>
        public void FSMSwitch(int kind)
        {
            if(fsmStack.Count > 0)
            {
                GetFSMLeaf(fsmStack.Pop())?.OnDisable();
            }
            var leaf = GetFSMLeaf(kind);
            leaf.OnEnable();
            fsmStack.Push(kind);
        }

        public override void AddChild(Leaf leaf, bool isActive)
        {
            AddFSMLeaf(leaf, isActive);
        }
       

    }
}
