using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    //体积光渲染
    [CreateAssetMenu(fileName = "VolumeLightRenderPass", menuName = "LPipeline/Passes/VolumeLightRenderPass")]
    public class VolumeLightRenderPass : RenderPass
    {
        private Material volumLightMaterial;
        private RenderTargetHandle volumeLightBackDepthTexture;

        private void OnEnable() {
            volumeLightBackDepthTexture.Init("_VolumeLightBackDepthTexture");
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            
            var cmd = CommandBufferPool.Get(nameof(VolumeLightRenderPass));
            var backDepthTextureDes = new RenderTextureDescriptor(data
            .renderWidth, data.renderHeight, RenderTextureFormat.Depth);
            backDepthTextureDes.sRGB = false;
            backDepthTextureDes.depthBufferBits = 32;
            cmd.GetTemporaryRT(volumeLightBackDepthTexture.id, backDepthTextureDes, FilterMode.Point);
            cmd.SetRenderTarget(data.activeCameraColorAttachment, volumeLightBackDepthTexture.Identifier());
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            

        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
        }

        
    }

}

