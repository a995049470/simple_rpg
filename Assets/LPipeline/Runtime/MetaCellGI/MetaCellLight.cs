using UnityEngine;

namespace LPipeline.Runtime
{
    [ExecuteAlways]
    public class MetaCellLight : MonoBehaviour
    {
        [SerializeField]
        private Color baseColor;
        [SerializeField]
        [Range(0, 24)]
        private float intensity;
        public Vector3 Position { get => this.transform.position; }
        public Vector3 LightColor 
        { 
            get
            {
                float t = Mathf.Pow(2, intensity);
                Vector3 color;
                color.x = t * baseColor.r;
                color.y = t * baseColor.g;
                color.z = t * baseColor.b;
                return color;
            }
        }

        private void OnEnable() {
            MetaCellLightManager.Instance.Add(this);
        }

        private void OnDisable() {
            MetaCellLightManager.Instance.Remove(this);
        }
    }
}

