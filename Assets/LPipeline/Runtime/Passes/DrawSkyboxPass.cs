using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "DrawSkyboxPass", menuName = "LPipeline/Passes/DrawSkyboxPass")]
    public class DrawSkyboxPass : RenderPass
    {
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            base.Execute(context, data);
            var cmd = CommandBufferPool.Get(nameof(DrawSkyboxPass));
            SetDefaultRenderTarget(cmd, context, data);
            context.DrawSkybox(data.camera);
            CommandBufferPool.Release(cmd);
        }
    }

}

