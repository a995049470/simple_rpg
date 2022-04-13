using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{

    [CreateAssetMenu(fileName = "IBLRenderPass", menuName = "LPipeline/Passes/IBLRenderPass")]
    public class IBLRenderPass : RenderPass
    {
        //环境光贴图
        // [SerializeField]
        // private Cubemap ambientLightMap;
        [SerializeField]
        private Material ambientMateril;

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            var cmd = CommandBufferPool.Get(nameof(IBLRenderPass));
            if(ambientMateril == null)
            {
                throw new System.Exception("没有环境光材质....");
            }
            SetDefaultRenderTarget(cmd, context, data);
            var fullScreenMesh = GetFullScreenQuad();
            cmd.DrawMesh(fullScreenMesh, Matrix4x4.identity, ambientMateril, 0);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

}

