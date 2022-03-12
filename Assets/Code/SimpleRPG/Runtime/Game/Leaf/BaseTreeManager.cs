using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public abstract class BaseTreeManager : SingleMono<BaseTreeManager>
    {
        protected void InitTree()
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
                    rootMono = leafMono;
                #else
                    rootMono = leafMono;
                    break;
                #endif
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
                var leafMono = stack.Pop();
                var tress = leafMono.Leaf as Tress;
                if(tress == null)
                {
                #if UNITY_EDITOR
                    if(leafMono.transform.childCount > 0)
                    {
                        Debug.Log($"{leafMono.name}为叶结点!子物体将失效!", leafMono.gameObject);
                    }
                #endif
                }
                else
                {
                    var childCount = leafMono.transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        var childLeafMono = leafMono.transform.GetChild(i).GetComponent<LeafMono>();
                        bool isActive = childLeafMono.gameObject.activeSelf;
                        var leaf = childLeafMono.Leaf;
                        leaf.SetParent(tress, isActive);
                        stack.Push(childLeafMono);
                    }
                }
                //清除缓存
                leafMono.ClearCache();
                

            }
        }
    }
}

