#ifndef LPIPELINE_BRDF_PASS
#define LPIPELINE_BRDF_PASS

// #define PI 3.1415926

//正态分布函数
float D_GGX_TR(float3 N, float3 H, float a)
{
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
    return nom / denom;
}
//a 取 roughness平方 会更自然
float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float nom   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / denom;
}

float GeometrySchlickGGX(float NdotV, float k)
{
    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}

//Smith法几何函数
float GeometrySmith(float3 N, float3 V, float3 L, float k)
{
    //奇怪的bug ??? max(dot(N, V), 0.0) 就出错.....
    float NdotV = max(dot(N, V), 0.00001);
    float NdotL = max(dot(N, L), 0.00001);
    
    float ggx1 = GeometrySchlickGGX(NdotV, k);
    float ggx2 = GeometrySchlickGGX(NdotL, k);
    
    return ggx1 * ggx2;
}

//菲涅尔方程
float3 FresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}
//考虑粗糙度的菲涅尔方程
float3 FresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(1.0 - roughness, F0) - F0) * pow(1.0 - cosTheta, 5.0);
}   

//来源 https://learnopengl-cn.github.io/07%20PBR/02%20Lighting/
float3 BDRF(float3 lightDir, float3 viewDir,
            float3 normal, float3 albedo,
            float3 lightColor, float roughness,
            float metallic, float ao)
{
    // float3 lightDir, viewDir, normal, albedo, lightColor;
    // float roughness, metallic, ao;
    //镜面高光项计算
    float3 f0 = 0.04f;
    f0 = lerp(f0, albedo, metallic); 
    float3 halfDir = normalize(lightDir + viewDir);
    
    float NDF = DistributionGGX(normal, halfDir, roughness);
    float G = GeometrySmith(normal, viewDir, lightDir, roughness);
    float HDotV = max(dot(halfDir, viewDir), 0);
    float3 F = FresnelSchlick(HDotV, f0);
    float3 ks = F;
    float3 kd = 1.0 - ks;
    kd *= 1.0 - metallic;
    float3 nominator = NDF * G * F;
    float NDotV = max(dot(normal, viewDir), 0);
    float NDotL = max(dot(normal, lightDir), 0);
    float denominator = 4.0 * NDotV * NDotL + 0.001;
    float3 specular = nominator / denominator;
    float3 a = (kd * albedo / PI + specular);
    float3 lo = a * lightColor * NDotL;
    //??? 好像有负数...
    return lo;
}

float3 Ambient_IBL(float3 normal, float3 viewDir,
                   float3 albedo, float3 irradiance,
                   float roughness, float metallic)
{
    float3 f0 = 0.04f;
    f0 = lerp(f0, albedo, metallic); 
    
    float NDotV = max(dot(normal, viewDir), 0);
    float3 ks = FresnelSchlickRoughness(NDotV, f0, roughness);
    float3 kd = 1.0 - ks;
    kd *= 1 - metallic;
    float3 diffuse = irradiance * albedo;
    float3 ambient = kd * diffuse;
    return ambient;
}


float RadicalInverse_VdC(uint bits) 
{
    bits = (bits << 16u) | (bits >> 16u);
    bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
    bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
    bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
    bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
    return float(bits) * 2.3283064365386963e-10; // / 0x100000000
}
// ----------------------------------------------------------------------------
float2 Hammersley(uint i, uint N)
{
    return float2(float(i)/float(N), RadicalInverse_VdC(i));
}  

float3 ImportanceSampleGGX(float2 Xi, float3 N, float roughness)
{
    float a = roughness*roughness;

    float phi = 2.0 * PI * Xi.x;
    float cosTheta = sqrt((1.0 - Xi.y) / (1.0 + (a*a - 1.0) * Xi.y));
    float sinTheta = sqrt(1.0 - cosTheta*cosTheta);

    // from spherical coordinates to cartesian coordinates
    float3 H;
    H.x = cos(phi) * sinTheta;
    H.y = sin(phi) * sinTheta;
    H.z = cosTheta;

    // from tangent-space vector to world-space sample vector
    float3 up        = abs(N.z) < 0.999 ? float3(0.0, 0.0, 1.0) : float3(1.0, 0.0, 0.0);
    float3 tangent   = normalize(cross(up, N));
    float3 bitangent = cross(N, tangent);

    float3 sampleVec = tangent * H.x + bitangent * H.y + N * H.z;
    return normalize(sampleVec);
}

float2 IntegrateBRDF(float NdotV, float roughness)
{
    float3 V;
    V.x = sqrt(1.0 - NdotV*NdotV);
    V.y = 0.0;
    V.z = NdotV;

    float A = 0.0;
    float B = 0.0;

    float3 N = float3(0.0, 0.0, 1.0);
    const uint SAMPLE_COUNT = 1024u;
    for(uint i = 0u; i < SAMPLE_COUNT; ++i)
    {
        float2 Xi = Hammersley(i, SAMPLE_COUNT);
        float3 H  = ImportanceSampleGGX(Xi, N, roughness);
        float3 L  = normalize(2.0 * dot(V, H) * H - V);

        float NdotL = max(L.z, 0.0);
        float NdotH = max(H.z, 0.0);
        float VdotH = max(dot(V, H), 0.0);

        if(NdotL > 0.0)
        {
            float G = GeometrySmith(N, V, L, roughness);
            float G_Vis = (G * VdotH) / (NdotH * NdotV);
            float Fc = pow(1.0 - VdotH, 5.0);

            A += (1.0 - Fc) * G_Vis;
            B += Fc * G_Vis;
        }
    }
    A /= float(SAMPLE_COUNT);
    B /= float(SAMPLE_COUNT);
    return float2(A, B);
}

#endif