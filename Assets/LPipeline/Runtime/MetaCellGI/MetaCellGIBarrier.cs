using System.Collections;
using NullFramework.Runtime;
using UnityEngine;

namespace LPipeline.Runtime
{


    [RequireComponent(typeof(MeshFilter))]
    [ExecuteAlways]
    public class MetaCellGIBarrier : MonoBehaviour
    {
        [SerializeField]
        private Mesh barrierMesh;
        public Mesh BarrierMesh { get => barrierMesh; }

        private void OnEnable() {
            if(barrierMesh == null)
            {
                var meshFilter = GetComponent<MeshFilter>();
                barrierMesh = meshFilter?.sharedMesh;
            }
            MetaCellGIBarrierManager.Instance.Add(this);
        }

        private void OnDisable() {
            MetaCellGIBarrierManager.Instance.Remove(this);
        }

    }
}

