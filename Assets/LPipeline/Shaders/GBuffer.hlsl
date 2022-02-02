#ifndef LPIPELINE_GBUFFER_PASS
#define LPIPELINE_GBUFFER_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD;
    float4 tangentOS : TANGENT;
    float3 normalOS : NORMAL;
};

struct Varyings
{
    float4 positionCS : POSITION;
    float2 uv : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float3 tangentWS : TEXCOORD2;
    float3 bitangentWS : TEXCOORD3;
    float3 positionWS : TEXCOORD4;
};

Texture2D _AlbedoTex; SamplerState sampler_AlbedoTex;
Texture2D _NormalTex; SamplerState sampler_NormalTex;
Texture2D _MetallicTex; SamplerState sampler_MetallicTex;
Texture2D _RoughnessTex; SamplerState sampler_RoughnessTex;
Texture2D _AOTex; SamplerState sampler_AOTex;

float3 _Albedo;
float _Metallic;
float _Roughness;
float _AO;

Varyings Vertex(Attributes i)
{
    Varyings o = (Varyings)0;
    o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
    o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
    o.uv = i.texcoord;
    VertexNormalInputs vni = GetVertexNormalInputs(i.normalOS, i.tangentOS);
    o.normalWS = vni.normalWS;
    o.tangentWS = vni.tangentWS;
    o.bitangentWS = vni.bitangentWS;
    return o;
}

void Fragment(Varyings i, 
out float3 gbuffer0 : SV_TARGET0, 
out float3 gbuffer1 : SV_TARGET1, 
out float3 gbuffer2 : SV_TARGET2)
{
    float3 albedo = _AlbedoTex.Sample(sampler_AlbedoTex, i.uv).rgb * _Albedo;
    float3 normalTS = _NormalTex.Sample(sampler_NormalTex, i.uv).rgb;
    normalTS = normalTS * 2.0 - 1.0;
    float3x3 t2w = float3x3(i.tangentWS, i.bitangentWS, i.normalWS);
    float3 normalWS = mul(normalTS, t2w);
    
    float metallic = _MetallicTex.Sample(sampler_MetallicTex, i.uv).r * _Metallic;
    float roughness = _RoughnessTex.Sample(sampler_RoughnessTex, i.uv).r * _Roughness;
    float ao = _AOTex.Sample(sampler_AOTex, i.uv).r * _AO;
    gbuffer0 = albedo;
    gbuffer1 = normalWS;
    gbuffer2 = float3(metallic, roughness, ao);
    //gbuffer2 = i.positionWS;
}


#endif