using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{

    [CreateAssetMenu(fileName = "LightingRenderPass", menuName = "LPipeline/Passes/LightingRenderPass")]
    public class LightingRenderPass : RenderPass
    {
        [SerializeField]
        private Material directionalLightMaterial;
        [SerializeField]
        private Material pointLightMaterial;
        //v4 x:亮度i y:二次项系数a z:一次项系数b w:常数项c  y = i / (ax^2 + bx + c)
        
        private static List<ShaderTagId> pointLightTags;

        public override void FirstCall()
        {
             pointLightTags = new List<ShaderTagId>()
            {
                new ShaderTagId("PointLight")
            };
            
        }


        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            
           
            var cmd = CommandBufferPool.Get(nameof(LightingRenderPass));
            SetDefaultRenderTarget(cmd, context, data);
            
            //收集所有视锥体内的灯光
            //开始平行光渲染
            var directionalLights = LightManager.Instance.GetVisibleDirectionalLights();
            if(directionalLightMaterial == null && directionalLights.Count > 0)
            {
                 throw new System.Exception("没有平行光材质 没法跑啊!");
            }
            for (int i = 0; i < directionalLights.Count; i++)
            {
                var directionalLight = directionalLights[i];
                var fullScreenMesh = GetFullScreenQuad();
                var lightColor = directionalLight.LightColor;
                var lightDirection = directionalLight.GetLightDirection();

                cmd.SetGlobalColor(ShaderUtils._LightColor, lightColor);
                cmd.SetGlobalVector(ShaderUtils._lightDirection, lightDirection);
                cmd.DrawMesh(fullScreenMesh, Matrix4x4.identity, directionalLightMaterial);
            }

            //开始点光渲染
            float intensityBias = Light_Point.IntensityBias;
            cmd.SetGlobalFloat(ShaderUtils._IntensityBias, intensityBias);
            var filterSettings = FilteringSettings.defaultValue;
            var drawSetting = CreateDrawingSettings(pointLightTags, data, SortingCriteria.CommonTransparent);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            context.DrawRenderers(data.cullingResults, ref drawSetting, ref filterSettings);
            
            // var pointLights = LightManager.Instance.GetVisiblePointLights();
            // for (int i = 0; i < pointLights.Count; i++)
            // {
            //     //设置光照参数
            //     var light = pointLights[i];
            //     var lightMesh = light.GetLightMesh();
            //     var lightColor = light.LightColor;
            //     var lightParameter = light.GetLightParameter();
            //     var lightPosition = light.transform.position;
            //     var lightMask = light.GetLightMask();
                
            //     cmd.SetGlobalVector(id_lightParameter, lightParameter);
            //     cmd.SetGlobalColor(id_lightColor, lightColor);
            //     cmd.SetGlobalVector(id_lightPosition, lightPosition);
            //     cmd.SetGlobalTexture(id_lgihtMask, lightMask);
                
            //     var matrix = light.GetLightMartix();
            //     cmd.DrawMesh(lightMesh, matrix, pointLightMaterial);
            // }
            // context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
        }

      
    }

}

