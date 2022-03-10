using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class LeafMono : MonoBehaviour
    {
        [SerializeField]
        private LeafData data;
        public LeafData Data { get => data; }
        [SerializeField]
        private bool isRoot = false;
        [SerializeField]
        private bool isFSMLeaf = false;
        public bool IsFSMLeaf { get => isFSMLeaf; }
        public bool IsRoot { get => isRoot;}
        private Leaf leaf;
        public Leaf Leaf
        {
            get
            {
                if(leaf == null) leaf = data?.CreateLeaf();
                return leaf;
            }
        }


        private void OnValidate() {
            if(data == null)
            {
                isRoot = false;
                return;
            } 
            isRoot = data is IRoot;
            isFSMLeaf = data is IFSMLeafData;
            this.name = data.name.Substring(0, data.name.Length - 4);
        }

        
    }
}

