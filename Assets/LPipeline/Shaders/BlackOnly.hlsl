#ifndef LPIPELINE_BLACK_ONLY_PASS
#define LPIPELINE_BLACK_ONLY_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD;
};

struct Varyings
{
    float4 positionCS : POSITION;
    float2 uv : TEXCOORD0;
};

Texture2D _AlbedoTex; SamplerState sampler_AlbedoTex;
Texture2D _AOTex; SamplerState sampler_AOTex;
float3 _Albedo;
float _AO;

Varyings Vertex(Attributes i)
{
    Varyings o = (Varyings)0;
    o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
    o.uv = i.texcoord;
    return o;
}

#define LIGHT_AMBINE 0.03

//加点基色 不至于太黑
float4 Fragment(Varyings i) : SV_TARGET
{
    float3 albedo = _AlbedoTex.Sample(sampler_AlbedoTex, i.uv).rgb * _Albedo;
    float ao = _AOTex.Sample(sampler_AOTex, i.uv).r * _AO;
    float3 color = LIGHT_AMBINE * ao * albedo;
    return float4(color, 1) ;
    
}


#endif