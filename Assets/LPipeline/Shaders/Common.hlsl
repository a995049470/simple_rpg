#ifndef LPIPELINE_COMMON_METHOD
#define LPIPELINE_COMMON_METHOD


float3 ScreenPosToWorldPosition(float2 uv, float depth, float4x4 invVP)
{
    float ndcz = UNITY_NEAR_CLIP_VALUE > 0 ? depth : depth * 2.0 - 1.0;
    float4 positionCS = float4(uv * 2.0 - 1.0, ndcz, 1.0);
#if UNITY_UV_STARTS_AT_TOP
	positionCS.y = -positionCS.y;
#endif
    float4 positionWS = mul(invVP, positionCS);
    return positionWS.xyz / positionWS.w;
}

#endif