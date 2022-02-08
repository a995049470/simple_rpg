using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace SimpleRPG.Runtime
{
    
    public struct CubeGBuffer
    {
        public Matrix4x4 rts;
        public Color albedo;
        public float metallic;
        public float roughness;
        public float ao;
        public const int Size = 4 * 23;
    }

    public class CubeRenderer : MonoBehaviour
    {
        [SerializeField]
        private TextAsset jsonAsset;
        //增量刷新
        private CubeGBuffer[] gbuffer;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Vector3 lastScale;
        
        private void InitBuffer()
        {
            
        }

        private void UpdateCubeBuffer()
        {

        }

    }

}
