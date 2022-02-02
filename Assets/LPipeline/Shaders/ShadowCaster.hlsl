#ifndef LPIPELINE_SHADOW_CASTER_PASS
#define LPIPELINE_SHADOW_CASTER_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
  
};

struct Varyings
{
    float4 positionCS : POSITION;
};

float4x4 _ShadowVP;

Varyings Vertex(Attributes i)
{
    Varyings o = (Varyings)0;
    float3 positionWS = TransformObjectToWorld(i.positionOS.xyz);
    o.positionCS = mul(_ShadowVP, float4(positionWS, 1));

    return o;
}

float4 Fragment(Varyings i) : SV_TARGET
{
    return i.positionCS.z;
}


#endif