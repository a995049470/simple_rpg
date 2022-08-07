using UnityEngine;

namespace LPipeline.Runtime
{
    public static class ShaderUtils
    {
        public static int _InvVP = Shader.PropertyToID("_InvVP");
        public static int _IntensityBias = Shader.PropertyToID("_IntensityBias");
        public static int _LightParameter = Shader.PropertyToID("_LightParameter");
        public static int _LightColor = Shader.PropertyToID("_LightColor");
        public static int _LightPosition = Shader.PropertyToID("_LightPosition");
        public static int _lightDirection = Shader.PropertyToID("_LightDirection");
        public static int _LightMask = Shader.PropertyToID("_LightMask");
        public static int id_depthTexture = Shader.PropertyToID("_DepthTexture");
        public static int _CubeGBuffer = Shader.PropertyToID("_CubeGBuffer");
        public static int _BarrierBuffer = Shader.PropertyToID("_BarrierBuffer");
        public static int _BlockNum = Shader.PropertyToID("_BlockNum");
        public static int _BlockSize = Shader.PropertyToID("_BlockSize");
        public static int _Origin = Shader.PropertyToID("_Origin");
        public static int _VerticesBuffer = Shader.PropertyToID("_VerticesBuffer");
        public static int _IndicesBuffer = Shader.PropertyToID("_IndicesBuffer");
        public static int _MatrixBuffer = Shader.PropertyToID("_MatrixBuffer");
        public static int _TriangleBarrierBuffer = Shader.PropertyToID("_TriangleBarrierBuffer");
        public static int _TriangleCount = Shader.PropertyToID("_TriangleCount");
        public static int _LightBuffer = Shader.PropertyToID("_LightBuffer");
        public static int _LightCount = Shader.PropertyToID("_LightCount");
        public static int _LightColorTexture = Shader.PropertyToID("_LightColorTexture");
        public static int _GlobalLightColorFrontTexture = Shader.PropertyToID("_GlobalLightColorFrontTexture");
        public static int _GlobalLightColorBackTexture = Shader.PropertyToID("_GlobalLightColorBackTexture");

        public static int _GlobalLightColorTexture = Shader.PropertyToID("_GlobalLightColorTexture");
        public static int _ShadowVP = Shader.PropertyToID("_ShadowVP");
        public static int _2DLightTex = Shader.PropertyToID("_2DLightTex");

    }
}