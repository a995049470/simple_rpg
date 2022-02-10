using UnityEngine;

namespace SimpleRPG.Runtime
{
    public struct CubeGBuffer
    {
        public Matrix4x4 localToWorldMatrix;
        public Vector3 albedo;
        public Vector3 normalTS;
        public float metallic;
        public float roughness;
        public float ao;
        public const int Size = 4 * (16 + 3 + 3 + 1 + 1 + 1);
    }
}
