using System.Collections;
using UnityEngine;
using LitJson;
using NullFramework.Runtime;
using Sirenix.OdinInspector;

namespace SimpleRPG.Runtime
{   
    public interface IColorChanger
    {
        Color GetColor();
        void SetColor(Color color); 
    }

    [ExecuteAlways]
    public class CubeRenderer : MonoBehaviour, IColorChanger
    {
        [SerializeField]
        private TextAsset data;
        

        //增量刷新
        private CubeGBuffer[] cubeBuffers;
        private static CubeGBuffer[] emptyGBuffer = new CubeGBuffer[0];
        //记录上一帧数据的变量必须再cubebuffer初始化时重新设置为单位量
        private Matrix4x4 lastLocalToWorldMatrix;
        private Color lastColor;
        [SerializeField]
        private Color cubeColor = Color.white;
        private const float bias = 0.0001f;
        
        

        private void Awake() {
            InitBuffer();
        }

        private void OnValidate() {
            InitBuffer();
        }
        
        private void OnEnable() {
            CubeRendererCenter.Instance.Add(this);
        }

        private void OnDisable() 
        {
            CubeRendererCenter.Instance.Remove(this);   
        }
        
        public CubeGBuffer[] GetCubeGBuffer()
        {
            var currentMatrix = this.transform.localToWorldMatrix;
            var currentColor = cubeColor;
            if (Mathf.Abs(currentMatrix[0, 0]) < bias) currentMatrix[0, 0] = bias;
            if (Mathf.Abs(currentMatrix[1, 1]) < bias) currentMatrix[3, 3] = bias;
            if (Mathf.Abs(currentMatrix[2, 2]) < bias) currentMatrix[3, 3] = bias;

          
            if (Mathf.Abs(currentColor.r) < bias) currentColor.r = bias;
            if (Mathf.Abs(currentColor.g) < bias) currentColor.g = bias;
            if (Mathf.Abs(currentColor.b) < bias) currentColor.b = bias;

            var offset_matrix = currentMatrix * lastLocalToWorldMatrix.inverse;
            lastLocalToWorldMatrix = currentMatrix;
            var offset_color = new Vector3(currentColor.r / lastColor.r, currentColor.g / lastColor.g, currentColor.b / lastColor.b);
            lastColor = currentColor;
            bool isMatrixChange = offset_matrix != Matrix4x4.identity;
            bool isColorChange = offset_color.x != 1 || offset_color.y != 1 || offset_color.z != 1;

            if(isColorChange || isMatrixChange)
            {
                //考虑job加速
                var len = cubeBuffers.Length;
                for (int i = 0; i < len; i++)
                {
                    ref CubeGBuffer buffer = ref cubeBuffers[i];
                    if(isMatrixChange)
                    {
                        buffer.localToWorldMatrix = offset_matrix * buffer.localToWorldMatrix;
                    }   
                    if(isColorChange)
                    {
                        buffer.albedo = new Vector3
                        (
                            buffer.albedo.x * offset_color.x, 
                            buffer.albedo.y * offset_color.y,
                            buffer.albedo.z * offset_color.z
                        );
                    }
                }
            }
            return cubeBuffers;
        }

        public int GetCubeCount()
        {
            return cubeBuffers.Length;
        }

        public void InitBuffer()
        {
            if(data == null)
            {
                cubeBuffers = emptyGBuffer;
            }
            else
            {
                cubeBuffers = StructUtility.BytesToArray<CubeGBuffer>(data.bytes);
            }
            //必须在这里把last变量初始化
            lastLocalToWorldMatrix = Matrix4x4.identity;
            lastColor = Color.white;
        }

        private void UpdateCubeBuffer()
        {
            
        }

        public Color GetColor()
        {
            return cubeColor;
        }

        public void SetColor(Color color)
        {
            cubeColor = color;
        }
    }

}
