using UnityEngine;
using UnityEngine.Rendering;
using SimpleRPG.Runtime;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "CubeGBufferRenderPass", menuName = "LPipeline/Passes/CubeGBufferRenderPass")]
    public class CubeGBufferRenderPass : RenderPass
    {
        [SerializeField]
        private Mesh cubeMesh;
        [SerializeField]
        private Material material;

        private RenderTargetHandle gbuffer0;
        //normal
        private RenderTargetHandle gbuffer1;
        //metallic + roughness + ao
        private RenderTargetHandle gbuffer2;
        //depth
        private RenderTargetHandle depthTexture;
        private int id_cubeGBuffer = Shader.PropertyToID("_CubeGBuffer");
        private ComputeBuffer cubeGbuffers;

        private void OnEnable()
        {
            gbuffer0.Init("_GBuffer0");
            gbuffer1.Init("_GBuffer1");
            gbuffer2.Init("_GBuffer2");
            depthTexture.Init("_DepthTexture");

        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            base.Execute(context, data);
            if (cubeMesh == null || material == null)
            {
                throw new System.Exception("没材质 没网格 渲个P!");
            }
            if(cubeGbuffers != null) cubeGbuffers.Release();
            CubeRendererCenter.Instance.GetBufferData(out cubeGbuffers, out var instanceCount);
            if (instanceCount == 0)

            {
                return;
            }
            var cmd = CommandBufferPool.Get(nameof(CubeGBufferRenderPass));
            var colorRTs = new RenderTargetIdentifier[]
            {
                gbuffer0.Identifier(),
                gbuffer1.Identifier(),
                gbuffer2.Identifier(),
            };
            cmd.SetRenderTarget(colorRTs, depthTexture.Identifier());
            
            material.SetBuffer(id_cubeGBuffer, cubeGbuffers);
            int subMeshIndex = 0;
            
            cmd.DrawMeshInstancedProcedural(cubeMesh, subMeshIndex, material, 0, instanceCount);
           
            cmd.SetRenderTarget(data.activeCameraColorAttachment, depthTexture.Identifier());
            cmd.DrawMeshInstancedProcedural(cubeMesh, subMeshIndex, material, 1, instanceCount);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
            
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            
        }
    }

}

