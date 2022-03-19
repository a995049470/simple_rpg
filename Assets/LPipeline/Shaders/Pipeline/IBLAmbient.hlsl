#ifndef LPIPELINE_IBL_AMBIENT_PASS
#define LPIPELINE_IBL_AMBIENT_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Common.hlsl"
#include "BRDF.hlsl"
struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD;
  
};

struct Varyings
{
    float4 positionCS : POSITION;
    float4 positionSS : TEXCOORD0;
};

TextureCube _MainTex; SamplerState sampler_MainTex;
Texture2D _DepthTexture; SamplerState sampler_DepthTexture;
Texture2D _GBuffer0; SamplerState sampler_GBuffer0;
Texture2D _GBuffer1; SamplerState sampler_GBuffer1;
Texture2D _GBuffer2; SamplerState sampler_GBuffer2;
float4x4 _InvVP;
float _AmbientIntensity;

Varyings Vertex(Attributes i)
{
    Varyings o = (Varyings)0;
    o.positionCS = i.positionOS;
#if UNITY_UV_STARTS_AT_TOP
    o.positionCS.y = -o.positionCS.y;
#endif
    o.positionSS = ComputeScreenPos(o.positionCS);
    
    return o;
}


float4 Fragment(Varyings i) : SV_TARGET
{
    float2 uvSS = i.positionSS.xy / i.positionSS.w;
    float depth = _DepthTexture.Sample(sampler_DepthTexture, uvSS).r;
    float3 albedo = _GBuffer0.Sample(sampler_GBuffer0, uvSS).rgb;
    float3 normalWS = normalize(_GBuffer1.Sample(sampler_GBuffer1, uvSS).rgb);
    float3 positionWS = ScreenPosToWorldPosition(uvSS, depth, _InvVP);
    float3 viewDir = normalize(GetCameraPositionWS() - positionWS);
    float3 gbuffer2 = _GBuffer2.Sample(sampler_GBuffer2, uvSS);
    float metallic = gbuffer2.x;
    float roughness = gbuffer2.y;
    float3 irradiance = _MainTex.Sample(sampler_MainTex, normalWS);
    float3 ambient = Ambient_IBL(normalWS, viewDir, albedo, irradiance, roughness, metallic);
    ambient *= _AmbientIntensity;
    return float4(ambient, 1);
}



#endif