using System.Collections;
using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [ExecuteInEditMode]
    public class LeafMono : MonoBehaviour
    {
        [SerializeField]
        private LeafData data;
        public LeafData Data { get => data; }
        [SerializeField]
        private bool isRoot = false;
        public bool IsRoot { get => isRoot;}
        private Leaf cacheLeaf;
        private ITRSSetter setter;
        public Leaf Leaf
        {
            get
            {
                if(cacheLeaf == null) cacheLeaf = data?.CreateLeaf();
                return cacheLeaf;
            }
        }

        public void ClearCache()
        {
            cacheLeaf = null;
        }

        private void OnValidate() {
            if(data == null)
            {
                isRoot = false;
                return;
            } 
            isRoot = data is IRootData;
            setter = data as ITRSSetter;
            this.name = data.name.Substring(0, data.name.Length - 4);
        }

        private void Update() {
            if(Application.isPlaying) return;
            if(setter != null)
            {
                setter.SetTRS(this.transform.position, this.transform.rotation, this.transform.lossyScale);
            }
        }
        
    }
}

