using System.Collections;
using UnityEngine;
using LitJson;
using NullFramework.Runtime;
using Sirenix.OdinInspector;

namespace SimpleRPG.Runtime
{

    [ExecuteAlways]
    public class CubeRenderer : MonoBehaviour
    {
        [SerializeField]
        private TextAsset data;
        //增量刷新
        private CubeGBuffer[] cubeBuffers;
        private static CubeGBuffer[] emptyGBuffer = new CubeGBuffer[0];
        private Matrix4x4 lastLocalToWorldMatrix;
        

        private void Awake() {
            InitBuffer();
        }

        private void OnValidate() {
            InitBuffer();
        }
        
        private void OnEnable() {
            CubeRendererCenter.Instance.Add(this);
        }

        private void OnDisable() {
            CubeRendererCenter.Instance.Remove(this);   
        }

        public CubeGBuffer[] GetCubeGBuffer()
        {
            var current = this.transform.localToWorldMatrix;
            var offset = current * lastLocalToWorldMatrix.inverse;
            lastLocalToWorldMatrix = current;
            if(offset != Matrix4x4.identity)
            {
                //考虑job加速
                var len = cubeBuffers.Length;
                for (int i = 0; i < len; i++)
                {
                    ref CubeGBuffer buffer = ref cubeBuffers[i];
                    buffer.localToWorldMatrix = offset * buffer.localToWorldMatrix;
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
            lastLocalToWorldMatrix = Matrix4x4.identity;
        }

        private void UpdateCubeBuffer()
        {
            
        }

    }

}
