using System.Collections;
using UnityEngine;

namespace LPipeline.Runtime
{
    [ExecuteAlways]
    public class VolumeFog : MonoBehaviour
    {
        [SerializeField]
        private Mesh mesh;
        public Mesh FogMesh { get => mesh; }

        [SerializeField]
        private int layer;
        
        public bool IsVaild()
        {
            return mesh != null;
        }

        public void OnEnable() {
            OnValidate(); 
        }

        public void OnDisable() {
            VolumeFogManager.Instance.Remove(this, layer);
        }

        private void OnValidate() {
            VolumeFogManager.Instance.Remove(this, layer);
            VolumeFogManager.Instance.Add(this, layer);
        }

        
        
        
    }
}

