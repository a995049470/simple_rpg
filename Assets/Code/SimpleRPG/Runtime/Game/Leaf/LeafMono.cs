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
        private GameObject previewObj;

       
        
        public void ClearCache()
        {
            cacheLeaf = null;
        }

        public void OnValidate() {
            if(data == null)
            {
                isRoot = false;
                return;
            } 
            isRoot = data is IRootData;
            setter = data as ITRSSetter;
            this.name = data.GetType().Name.Replace("Data", "");
            //编辑器下会产生预览的物体
            if(!Application.isPlaying)
            {
                if(previewObj != null) DestroyImmediate(previewObj);
            }
        }
        

        private void Update() {
            if(Application.isPlaying) return;
            if(setter != null)
            {
                setter.SetTRS(this.transform.position, this.transform.rotation, this.transform.lossyScale);
            }
            if(previewObj != null)
            {
                previewObj.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            }
        }

        public void SetData(LeafData _data)
        {
            data = _data;
            OnValidate();
        }
        
    }
}

