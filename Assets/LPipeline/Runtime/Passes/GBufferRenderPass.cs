using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    //暂定三张GBuffer + depthTexture
    //1.depth 2.albedo 3.normal 4.metallic + roughness + ao
    [CreateAssetMenu(fileName = "GBufferRenderPass", menuName = "LPipeline/Passes/GBufferRenderPass")]
    public class GBufferRenderPass : RenderPass
    {
        //albedo
        private RenderTargetHandle gbuffer0;
        //normal
        private RenderTargetHandle gbuffer1;
        //metallic + roughness + ao
        private RenderTargetHandle gbuffer2;
        //depth
        private RenderTargetHandle depthTexture;
        
        private List<ShaderTagId> gbufferTagList;     
        private List<ShaderTagId> blackTagList;
        


        public override void FirstCall(){
            gbuffer0.Init("_GBuffer0");
            gbuffer1.Init("_GBuffer1");
            gbuffer2.Init("_GBuffer2");
            depthTexture.Init("_DepthTexture");
            gbufferTagList = new List<ShaderTagId>()
            {
                new ShaderTagId("GBuffer"),
            };
            blackTagList = new List<ShaderTagId>()
            {
                new ShaderTagId("BlackOnly"),
            };
        }
        
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
          
            
            //输出gbuffer
            var des0 = new RenderTextureDescriptor(data.renderWidth, data.renderHeight, RenderTextureFormat.ARGBHalf);
            des0.sRGB = false;
            var des1 = new RenderTextureDescriptor(data.renderWidth, data.renderHeight, RenderTextureFormat.ARGBHalf);
            des1.sRGB = false;
            
            var des2 = new RenderTextureDescriptor(data.renderWidth, data.renderHeight, RenderTextureFormat.ARGBHalf);
            des2.sRGB = false;
            var depthTextureDes = new RenderTextureDescriptor(data.renderWidth, data.renderHeight, RenderTextureFormat.Depth, 32);
            depthTextureDes.sRGB = false;

            var cmd = CommandBufferPool.Get(nameof(GBufferRenderPass));
            cmd.GetTemporaryRT(gbuffer0.id, des0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(gbuffer1.id, des1, FilterMode.Point);
            cmd.GetTemporaryRT(gbuffer2.id, des2, FilterMode.Point);
            cmd.GetTemporaryRT(depthTexture.id, depthTextureDes, FilterMode.Point);
            //data.activeCameraDepthAttachment = depthTexture.Identifier();
            var colorRTs = new RenderTargetIdentifier[]
            {
                gbuffer0.Identifier(),
                gbuffer1.Identifier(),
                gbuffer2.Identifier(),
            };
             //把depthTexture当作激活的深度缓冲
            
            cmd.SetRenderTarget(colorRTs, depthTexture.Identifier());
            cmd.ClearRenderTarget(true, true, Color.black, 1.0f);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            var drawSetting  = CreateDrawingSettings(gbufferTagList, data, SortingCriteria.CommonOpaque);
            var filteringSettings = FilteringSettings.defaultValue;
            context.DrawRenderers(data.cullingResults, ref drawSetting, ref filteringSettings);
            
            //再跑一个输出纯黑的的pass
            cmd.SetRenderTarget(data.activeCameraColorAttachment, depthTexture.Identifier());
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            //SetDefaultRenderTarget(cmd, context, data);
            var blackDrawSetting = CreateDrawingSettings(blackTagList, data, SortingCriteria.CommonOpaque);
            context.DrawRenderers(data.cullingResults, ref blackDrawSetting, ref filteringSettings);
            

            CommandBufferPool.Release(cmd);
            data.AddActiveRTId(depthTexture.id);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(gbuffer0.id);
            cmd.ReleaseTemporaryRT(gbuffer1.id);
            cmd.ReleaseTemporaryRT(gbuffer2.id);
            cmd.ReleaseTemporaryRT(depthTexture.id);
        }

        
    }

}

