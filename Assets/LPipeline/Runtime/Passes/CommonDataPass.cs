using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    //赋值一些常用的数据
    [CreateAssetMenu(fileName = "CommonDataPass", menuName = "LPipeline/Passes/CommonDataPass")]
    public class CommonDataPass : RenderPass
    {
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            var cmd = CommandBufferPool.Get(nameof(CommonDataPass));
            var vp = data.gpuProjectionMatrix * data.viewMatrix;
            var invVp = vp.inverse;
            cmd.SetGlobalMatrix(ShaderUtils._InvVP, invVp);
            float intensityBias = Light_Point.IntensityBias;
            cmd.SetGlobalFloat(ShaderUtils._IntensityBias, intensityBias);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

}

