using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class TreeManager : SingleMono<TreeManager>
    {
        private void Start() {
            InitTree();
        }

        private void Update() {
            Root.Instance.Update(Time.deltaTime);
        }

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
                        throw new System.Exception($"{leafMono.name}为叶结点!子物体将失效!");
                    }
                #endif
                    continue;
                }
                var childs = leafMono.GetComponentsInChildren<LeafMono>(false);
                var childCount = leafMono.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var childLeafMono = leafMono.transform.GetChild(i).GetComponent<LeafMono>();
                    bool isActive = childLeafMono.gameObject.activeSelf;
                    var leaf = childLeafMono.Leaf;
                    leaf.SetParent(tress);
                    if(childLeafMono.Data is IFSMLeaf fsmLeaf)
                    {
                        tress.AddFSMLeaf(fsmLeaf.GetLeafKind(), leaf, isActive);
                    }
                    else
                    {
                        if(isActive)
                        {
                            leaf.OnEnter();
                        }
                    }
                    stack.Push(childLeafMono);
                }
                

            }
        }
        
    }
}

