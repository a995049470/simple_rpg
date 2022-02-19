Shader "LPipeline/Light_Point"
{
    Properties
    {
        [HideInInspector]_LightColor("_LightColor", color) = (0, 0, 0, 0)
        [HideInInspector]_LightParameter("_LightParameter", vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        pass
        {
            Name "Light_Point"
            Tags
            {
                "RenderType"="Opaque"
                "LightMode"="PointLight"
            }
            ZWrite Off
            //ZTest Less
            //Cull back
            ZTest Less
            Blend One One
            Cull Front
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Light_Point.hlsl"

            ENDHLSL
        }

        
        
        
    }
}
