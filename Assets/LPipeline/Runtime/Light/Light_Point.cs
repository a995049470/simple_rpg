using System.Collections;
using UnityEngine;

namespace LPipeline
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [ExecuteAlways]
    public class Light_Point : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 16)]
        private float intensityPow = 1;
        [SerializeField]
        private Color lightColor = Color.white;

        public Color LightColor { get => lightColor; }

        [SerializeField]
        [Range(0.1f, 32)]
        private float lightRaidus = 1.0f;
        [SerializeField]
        [Range(0.0f, 1.0f)]
        //光强衰减系数
        private float lightAttenuation = 1.0f;
        //光照范围的近似mesh 先拿cube用一下
        //[SerializeField]
        private Mesh lightMesh;
        [SerializeField] 
        private static Mesh defalutMesh;
        
        //[SerializeField]
        private Cubemap lightMask;
        private static Cubemap defalutTexture; 
        

        private MaterialPropertyBlock materialPropertyBlock;
        private Renderer cacheRenderer;
        private static int id_lightParameter = Shader.PropertyToID("_LightParameter");
        private static int id_lightColor = Shader.PropertyToID("_LightColor");
        private static int id_lightPosition = Shader.PropertyToID("_LightPosition");
        private static int id_lightDirection = Shader.PropertyToID("_LightDirection");
        private static int id_lightMask = Shader.PropertyToID("_LightMask");

        //微小的数
        public const float IntensityBias = 0.1f;

        private void OnEnable() {
            SetMaterialPropertyBlock();
            LightManager.Instance.AddLight(this);
        }

        private void OnDisable() {
            LightManager.Instance.RemoveLight(this);
        }

        private void OnValidate() {
           SetMaterialPropertyBlock();
        }

        private void SetMaterialPropertyBlock()
        {
            if(cacheRenderer == null)
            {
                cacheRenderer = this.GetComponent<Renderer>();
                if(cacheRenderer == null)
                {
                    return;
                }
            }
            this.transform.localScale = 2.0f * lightRaidus * Vector3.one;
            var lightColor = this.LightColor;
            var lightParameter = this.GetLightParameter();
            var lightPosition = this.transform.position;
            materialPropertyBlock = materialPropertyBlock ?? new MaterialPropertyBlock();
            materialPropertyBlock.SetColor(id_lightColor, lightColor);
            materialPropertyBlock.SetVector(id_lightParameter, lightParameter);
            //materialPropertyBlock.SetTexture(id_lightMask, GetLightMask());
            cacheRenderer.SetPropertyBlock(materialPropertyBlock);
            
        }

        public Cubemap GetLightMask()
        {   
            if(lightMask == null)
            {
                if(defalutTexture == null)
                {
                    defalutTexture = new Cubemap(1, TextureFormat.ARGB32, 1);
                    var colors = new Color[] { Color.white };
                    defalutTexture.SetPixels(colors, CubemapFace.NegativeX);
                    defalutTexture.SetPixels(colors, CubemapFace.NegativeY);
                    defalutTexture.SetPixels(colors, CubemapFace.NegativeZ);
                    defalutTexture.SetPixels(colors, CubemapFace.PositiveX);
                    defalutTexture.SetPixels(colors, CubemapFace.PositiveY);
                    defalutTexture.SetPixels(colors, CubemapFace.PositiveZ);
                    defalutTexture.Apply();
                }
                return defalutTexture;
            }
            return lightMask;
        }

        /// <summary>
        /// 光照参数 y = i / (ax^2 + bx + c)
        /// </summary>
        /// <returns>0:i 1:a 2:b 3:c</returns>
        public Vector4 GetLightParameter()
        {
            var para = Vector4.zero;
            float intensity = Mathf.Pow(2, intensityPow) - 1;
            para[0] = intensity;
            // intensity / (a * r * r + b * r + c) = tinyNum
            // a * r * r + b * r = intensity / tinyNum - c
            float c = 1;
            float i = 1.0f;
            float a_max = (i / IntensityBias - c) / (lightRaidus * lightRaidus);
            float a_min = 0;
            float a = Mathf.Lerp(a_min, a_max, lightAttenuation);
            float b =  (i / IntensityBias - c - a * lightRaidus * lightRaidus) / lightRaidus;
            para[1] = a;
            para[2] = b;
            para[3] = c;
            return para;
        }

        public Matrix4x4 GetLightMartix()
        {
            return Matrix4x4.TRS(this.transform.position, this.transform.rotation, 2.0f * lightRaidus * Vector3.one);
        }

        public Mesh GetLightMesh()
        {
            if (lightMesh == null)
            {
                if (defalutMesh == null)
                {
                    defalutMesh = new Mesh();
                    float radius = 0.5f;
                    Vector3[] vertices = new Vector3[]
                    {
                        new Vector3(-1, -1, -1) * radius,
                        new Vector3( 1, -1, -1) * radius,
                        new Vector3( 1, -1,  1) * radius,
                        new Vector3(-1, -1,  1) * radius,

                        new Vector3(-1,  1, -1) * radius,
                        new Vector3( 1,  1, -1) * radius,
                        new Vector3( 1,  1,  1) * radius,
                        new Vector3(-1,  1,  1) * radius,
                    };
                    lightMesh.vertices = vertices;
                    var indices = new int[]
                    {
                        3, 2, 1, 0, 3, 1,
                        5, 6, 7, 5, 7, 4,
                        2, 6, 5, 1, 2, 5,
                        4, 7, 3, 0, 4, 3,
                        5, 4, 0, 1, 5, 0,
                        2, 3, 7, 2, 7, 6
                    };
                    lightMesh.SetIndices(indices, MeshTopology.Triangles, 0);
                }
                return defalutMesh;
            }
            return lightMesh;
        }

        
    }
}

