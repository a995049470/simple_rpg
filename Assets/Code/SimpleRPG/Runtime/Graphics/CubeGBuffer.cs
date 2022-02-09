using UnityEngine;

namespace SimpleRPG.Runtime
{
    public struct CubeGBuffer
    {
        public Matrix4x4 localToWorldMatrix;
        public Color albedo;
        public float metallic;
        public float roughness;
        public float ao;
        public const int Size = 4 * 23;
    }

}
