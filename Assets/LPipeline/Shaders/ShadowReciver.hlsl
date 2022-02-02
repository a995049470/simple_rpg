#ifndef LPIPELINE_SHADOW_RECIVER_PASS
#define LPIPELINE_SHADOW_RECIVER_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

Texture2D _ShadowMap; SamplerState sampler_ShadowMap;
//TODO : PCSS 软阴影
float Visiable(float3 positionWS)
{
    float4 positionCS_Shadow = mul(_ShadowVP, float4(positionWS, 1.0));
    float2 uvSS_Shadow = ComputeScreenPos(positionCS_Shadow).xy;
    float depth_Shadow = _ShadowMap.Sample(sampler_ShadowMap, uvSS_Shadow);
    return positionCS_Shadow.z > depth_Shadow ? 1 : 0.2f;
}

#endif