#ifndef LPIPELINE_BRDF_PASS
#define LPIPELINE_BRDF_PASS

#define PI 3.1415926

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
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx1 = GeometrySchlickGGX(NdotV, k);
    float ggx2 = GeometrySchlickGGX(NdotL, k);

    return ggx1 * ggx2;
}

//菲涅尔方程
float3 FresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
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
    
    float3 lo = (kd * albedo / PI + specular) * lightColor * NDotL;
    return lo;

}

#endif