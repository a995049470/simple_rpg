using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "Render2DLightPass", menuName = "LPipeline/Passes/Render2DLightPass")]
    public class Render2DLightPass : RenderPass
    {
        //2d障碍相关
        [SerializeField]
        private int barrierDownsampleCount = 1;
        private RenderTargetHandle barrierTex;
        private List<ShaderTagId> barrierTags;
        //2d灯光相关
        [SerializeField]
        private int lightDownsampleCount = 1;
        [SerializeField]
        private int lightTexWidth = 256;
        [SerializeField]
        private int lightTexHeight = 128; 
        private RenderTargetHandle originLightTex;
        private RenderTargetHandle lightFrontTex;
        private RenderTargetHandle lightBackTex;
        [SerializeField]
        private Material lightUpdateMaterial;
        // [SerializeField]
        // private Material lightDrawMaterial;
        [SerializeField]
        [Range(1, 64)]
        private int lightUpdateCount = 1;
        private List<ShaderTagId> lightTags;

        public override void FirstCall()
        {
            base.FirstCall();
            barrierTex.Init("_2DLightBarrierTex");
            originLightTex.Init("_OriginLightTex");
            lightFrontTex.Init("_LightFrontTex");
            lightBackTex.Init("_LightBackTex");
            barrierTags = new List<ShaderTagId>()
            {
                new ShaderTagId("LightBarrier")
            };
            lightTags = new List<ShaderTagId>()
            {
                new ShaderTagId("Light2D"),
            };


        }

        private void SwapLightTex()
        {
            var temp = lightFrontTex;
            lightFrontTex = lightBackTex;
            lightBackTex = temp;
        }


        private void RenderBarrier(ScriptableRenderContext context, RenderData data, CommandBuffer cmd)
        {
            var width = Mathf.Max(1, data.colorDescriptor.width / barrierDownsampleCount);
            var height = Mathf.Max(1, data.colorDescriptor.height / barrierDownsampleCount);
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGBHalf);
            cmd.GetTemporaryRT(barrierTex.id, des, FilterMode.Bilinear);
            cmd.SetRenderTarget(barrierTex.Identifier());
            cmd.ClearRenderTarget(false, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            var drawSetting = CreateDrawingSettings(barrierTags, data, SortingCriteria.SortingLayer);
            context.DrawRenderers(data.cullingResults, ref drawSetting, ref defaultFilteringSettings);
        }

        private void RenderLights(ScriptableRenderContext context, RenderData data, CommandBuffer cmd)
        {
            // var width = Mathf.Max(1, data.colorDescriptor.width / lightDownsampleCount);
            // var height = Mathf.Max(1, data.colorDescriptor.height / lightDownsampleCount);
            var width = lightTexWidth;
            var height = lightTexHeight;
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGBHalf);
            cmd.GetTemporaryRT(lightFrontTex.id, des, FilterMode.Bilinear);
            cmd.GetTemporaryRT(lightBackTex.id, des, FilterMode.Bilinear);
            
            des.width = data.colorDescriptor.width;
            des.height = data.colorDescriptor.height;
            cmd.GetTemporaryRT(originLightTex.id, des, FilterMode.Bilinear);
            
            var drawSetting = CreateDrawingSettings(lightTags, data, SortingCriteria.SortingLayer);
            cmd.SetRenderTarget(originLightTex.id);
            cmd.ClearRenderTarget(false, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            context.DrawRenderers(data.cullingResults, ref drawSetting, ref defaultFilteringSettings);
            cmd.Blit(originLightTex.Identifier(), lightFrontTex.Identifier());
            for (int i = 0; i < lightUpdateCount; i++)
            {
                cmd.Blit(lightFrontTex.Identifier(), lightBackTex.Identifier(), lightUpdateMaterial);
                SwapLightTex();
            }
            cmd.SetGlobalTexture(ShaderUtils._2DLightTex, lightFrontTex.Identifier());
            // cmd.SetRenderTarget(data.activeCameraColorAttachment, data.activeCameraDepthAttachment);
            // cmd.DrawMesh(GetFullScreenQuad(), Matrix4x4.identity, lightDrawMaterial);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
#if UNITY_EDITOR
            if (lightUpdateMaterial == null /*|| lightDrawMaterial == null*/)
            {
                Debug.LogError("缺少材质");
                return;
            }
#endif
            var cmd = CommandBufferPool.Get(nameof(Render2DLightPass));
            RenderBarrier(context, data, cmd);
            RenderLights(context, data, cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(barrierTex.id);
            cmd.ReleaseTemporaryRT(lightFrontTex.id);
            cmd.ReleaseTemporaryRT(lightBackTex.id);
        }
    }

}

