Shader "LPipeline/SampleLit2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        //_Color("Color", color) = (1, 1, 1, 1)
        
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Pass
        {
            Tags 
            {
                "LightMode" = "LPipeline"
            }
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets\LPipeline\Shaders\Pipeline\Lit2DUtility.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 uv_lit : TEXCOORD1;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            //float4 _Color;

            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                o.uv = v.uv;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.color = v.color;
                o.uv_lit = ComputeScreenPos(o.positionCS);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = _MainTex.Sample(sampler_MainTex, i.uv);
                float3 lightColor = Get2DLightColor(i.uv_lit.xy / i.uv_lit.w);
                col *= i.color;
                col.rgb *= lightColor;
                return col;
            }
            ENDHLSL
        }
    }
}
