using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class TreeManager : SingleMono<TreeManager>
    {
        public void InitTree()
        {
            var leafMonos = GameObject.FindObjectsOfType<LeafMono>();
            LeafMono rootMono = null;
            foreach (var leafMono in leafMonos)
            {
                if(leafMono.IsRoot && leafMono.Data != null)
                {
                #if UNITY_EDITOR
                    if(rootMono != null )
                    {
                        throw new System.Exception("不允许存在超过一个的Root!");
                    }
                #endif
                    rootMono = leafMono;
                    continue;
                }
                
            }
            #if UNITY_EDITOR
                if(rootMono == null)
                {
                    throw new System.Exception("Root节点为空!");
                }
            #endif
            Stack<LeafMono> stack = new Stack<LeafMono>();
            stack.Push(rootMono);
            while (stack.Count > 0)
            {
                
            }
        }
        
    }
}

