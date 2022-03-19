using UnityEngine;

namespace LPipeline
{
    [ExecuteAlways]
    public class Light_Directional : MonoBehaviour
    {
        [Header("暂时只能存在一个全局光 需要指定全局光产生的阴影贴图")]
        [SerializeField]
        [Range(0, 2)]
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

