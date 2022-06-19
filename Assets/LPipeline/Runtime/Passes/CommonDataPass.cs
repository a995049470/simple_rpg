using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    //赋值一些常用的数据
    [CreateAssetMenu(fileName = "CommonDataPass", menuName = "LPipeline/Passes/CommonDataPass")]
    public class CommonDataPass : RenderPass
    {
        private int id_invVP = Shader.PropertyToID("_InvVP");
        private int id_intensityBias = Shader.PropertyToID("_IntensityBias");
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            var cmd = CommandBufferPool.Get(nameof(CommonDataPass));
            var vp = data.gpuProjectionMatrix * data.viewMatrix;
            var invVp = vp.inverse;
            cmd.SetGlobalMatrix(id_invVP, invVp);
            float intensityBias = Light_Point.IntensityBias;
            cmd.SetGlobalFloat(id_intensityBias, intensityBias);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

}

