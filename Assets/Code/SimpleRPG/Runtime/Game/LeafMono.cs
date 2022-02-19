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
        public bool IsRoot { get => isRoot;}

        private void OnValidate() {
            if(data == null) isRoot = false;
            isRoot = data.GetType().Name.ToLower().Contains("root");
        }
    }
}

