#ifndef LPIPELINE_LIGHT_POINT_PASS
#define LPIPELINE_LIGHT_POINT_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "Common.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD;
};

struct Varyings
{
    float4 positionCS : POSITION;
    float4 positionSS : TEXCOORD0;
    float2 uv : TEXCOORD1;
};

Texture2D _DepthTexture; SamplerState sampler_DepthTexture;
Texture2D _GBuffer0; SamplerState sampler_GBuffer0;
Texture2D _GBuffer1; SamplerState sampler_GBuffer1;
Texture2D _GBuffer2; SamplerState sampler_GBuffer2;
TextureCube _LightMask; SamplerState sampler_LightMask;

float4 _LightParameter;
float4 _LightColor;
float4x4 _InvVP;
float _IntensityBias;

#include "BRDF.hlsl"

Varyings Vertex(Attributes i)
{
    Varyings o = (Varyings)0;
    o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
    o.positionSS = ComputeScreenPos(o.positionCS);
    o.uv = i.texcoord;
    return o;
}



float GetLightIntensity(float dis)
{
    float intensity = 1.0 / (_LightParameter.y * dis * dis + _LightParameter.z * dis + _LightParameter.w);
    intensity = max(intensity, _IntensityBias);
    intensity = (intensity - _IntensityBias) / (1.0 - _IntensityBias) * _LightParameter.x;
    
    return intensity; 
}

#define PI 3.1415926
float2 PositionToUV(float3 position)
{
    float r = sqrt(position.x * position.x + position.y * position.y + position.z * position.z);
    float u = atan(position.y / position.x) / 2.000f / PI;
    float v = asin(position.z / r) / PI + 0.500f;
    return float2(u, v);
}

float4 Fragment(Varyings i) : SV_TARGET
{
    float2 uvSS = i.positionSS.xy / i.positionSS.w;
    float depth = _DepthTexture.Sample(sampler_DepthTexture, uvSS).r;
    float3 albedo = _GBuffer0.Sample(sampler_GBuffer0, uvSS).rgb;
    float3 normalWS = normalize(_GBuffer1.Sample(sampler_GBuffer1, uvSS).rgb);
    float3 positionWS = ScreenPosToWorldPosition(uvSS, depth, _InvVP);
    float3 lightPosition = float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3], UNITY_MATRIX_M[2][3]);
    float3 lightDir = normalize(lightPosition - positionWS);
    float dis = distance(positionWS, lightPosition);
    float intensity = GetLightIntensity(dis);
    
    float ldotn = dot(normalWS, lightDir);
    ldotn = max(0, ldotn);
    float3 viewDir = normalize(GetCameraPositionWS() - positionWS);

    float3 gbuffer2 = _GBuffer2.Sample(sampler_GBuffer2, uvSS);
    float metallic = gbuffer2.x;
    float roughness = gbuffer2.y;
    float ao = gbuffer2.z;

    float3 uv_mask = normalize(TransformWorldToObject(positionWS));
    float3 mask = _LightMask.Sample(sampler_LightMask, uv_mask).rgb;
    
    float3 lightColor = _LightColor * intensity * mask;
    
    float3 brdf = BDRF(lightDir, viewDir, normalWS, albedo, lightColor, roughness, metallic, ao);
    
    float3 color = brdf;
    return float4(color, 1);
}



#endif