using UnityEngine;

namespace LPipeline
{
    [ExecuteAlways]
    public class Light_Directional : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        private float intensity;
        [SerializeField]
        private Color lightColor;
        public Color LightColor { get => intensity * lightColor; }

        public Vector3 GetLightDirection()
        {
            return -this.transform.forward;
        }

        private void OnEnable() {
            LightManager.Instance.AddLight(this);
        }

        private void OnDisable() {
            LightManager.Instance.RemoveLight(this);
        }
    }
}

