using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "ToneMappingPass", menuName = "LPipeline/Passes/ToneMappingPass")]
    public class ToneMappingPass : RenderPass
    {
        [SerializeField]
        private Material toneMappingMaterail;
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            if(toneMappingMaterail == null)
            {
                throw new System.Exception("没ToneMaping材质啊!");
            }
            var tempDes = data.colorDescriptor;
            var cmd = CommandBufferPool.Get(nameof(ToneMappingPass));
            cmd.GetTemporaryRT(TempRT.id, tempDes, FilterMode.Bilinear);
            cmd.Blit(data.activeCameraColorAttachment, TempRT.Identifier(), toneMappingMaterail);
            cmd.Blit(TempRT.Identifier(), data.activeCameraColorAttachment);
            cmd.ReleaseTemporaryRT(TempRT.id);
            context.ExecuteCommandBuffer(cmd);
        }

       
    }

}

