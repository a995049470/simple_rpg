Shader "LPipeline/MetaCellLight"
{
    Properties
    {
        
    }
    SubShader
    {
        //产生shadowMap
        pass
        {
            Name "MetaCellLit"
            Tags
            {
                "RenderType"="Opaque"
                "LightMode"="MetaCellLit"
            }
            ZWrite On
            ZTest Less
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex Vertex
            #pragma fragment Fragment
            //#pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                
            };

            struct Varyings
            {
                float4 positionCS : POSITION;
                float3 cellLightUV : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            float3 _Origin;
            float3 _BlockNum;
            float3 _BlockSize;
            Texture3D _GlobalLightColorTexture;
            SamplerState sampler_GlobalLightColorTexture;
            
            Varyings Vertex(Attributes i)
            {
                Varyings o = (Varyings)0;
                float3 positionWS = TransformObjectToWorld(i.positionOS);
                float3 cellLightUV = (positionWS - _Origin) / (_BlockNum * _BlockSize);
                o.positionWS = positionWS;
                o.cellLightUV = cellLightUV;
                o.positionCS = TransformObjectToHClip(i.positionOS);
                return o;
            }

            float4 Fragment(Varyings i) : SV_TARGET
            {
                float3 uv = (i.positionWS - _Origin) / (_BlockNum * _BlockSize);
                //return float4(uv, 1);
                float3 lightColor = _GlobalLightColorTexture.Sample(sampler_GlobalLightColorTexture, i.cellLightUV).xyz;
                //lightColor = step(0, i.cellLightUV - .5);
                return float4(lightColor, 1);
            }
            ENDHLSL
        }
    }
}
