Shader "LPipeline/IBLAmbient"
{
    Properties
    {
        _MainTex("MainTex", Cube) = "" {}
        _AmbientIntensity("环境光强度", Range(0, 2)) = 1
        
    }
    SubShader
    {
        
        
        pass
        {
            Name "IBLAmbient"
            Tags
            {
                "RenderType"="Opaque"
                "LightMode"="IBLBlur"
            }
            ZWrite Off
            ZTest Always
            Blend One One
            Cull Back
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "IBLAmbient.hlsl"
            ENDHLSL
        }
    }
}