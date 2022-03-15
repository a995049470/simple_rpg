Shader "LPipeline/Test1"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _CubeMap("CubeMap", Cube) = "" {}
    }
    SubShader
    {
        Tags 
        {
            "Queue"="Transparent"
        }
        pass
        {
            Tags 
            {
                "RenderType" = "Transparent" 
                "LightMode" = "Universal2D" 
            }
            ZWrite Off
            ZTest Less
            //Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex Vertex
            #pragma fragment Fragment



            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD;
            };

            struct Varyings
            {
                float4 positionCS : POSITION;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            Texture2D _MainTex; SamplerState sampler_MainTex;
            TextureCube _CubeMap; SamplerState sampler_CubeMap;
            
            SAMPLER(sampler_textureName);
            float4 _Color;

            #define PI 3.1415926
            float2 PositionToUV(float3 position)
            {
                float r = sqrt(position.x * position.x + position.y * position.y + position.z * position.z);
                float u = atan(position.y / position.x) / 2.000f / PI;
                float v = asin(position.z / r) / PI + 0.500f;
                return float2(u, v);
            }

            Varyings Vertex(Attributes i)
            {
                Varyings o = (Varyings)0;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldDir(i.normalOS);
                //o.uv = PositionToUV(normalize(i.positionOS));
                o.uv = i.texcoord;
                o.positionWS = TransformObjectToWorld(i.positionOS);
                return o;
            }


            float4 Fragment(Varyings i) : SV_TARGET
            {
                float4 color = _MainTex.Sample(sampler_MainTex, i.uv);
                return color;
            }
            ENDHLSL

        }
    }
}
