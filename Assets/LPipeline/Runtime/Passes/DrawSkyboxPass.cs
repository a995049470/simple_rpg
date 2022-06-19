using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "DrawSkyboxPass", menuName = "LPipeline/Passes/DrawSkyboxPass")]
    public class DrawSkyboxPass : RenderPass
    {
        [SerializeField]
        private Material skyboxMaterial;
        [SerializeField]
        private Mesh cube;
        private static int id_depthTexture = Shader.PropertyToID("_DepthTexture");

        
        

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            
            var cmd = CommandBufferPool.Get(nameof(DrawSkyboxPass));
           
            //要那gbuffer里的depthTexture当作深度目标
           // cmd.SetRenderTarget(data.activeCameraColorAttachment, Shader.PropertyToID("_DepthTexture"));
            SetRenderTarget(cmd, context, data, -1, id_depthTexture);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            if(skyboxMaterial == null || cube == null)
            {
                //使用unity自带画天空盒方法
                context.DrawSkybox(data.camera);
            }
            else
            {
                var position = data.camera.transform.position;
                var rotation = Quaternion.identity;
                var scale = Vector3.one;
                var matrix = Matrix4x4.TRS(position, rotation, scale);
                cmd.DrawMesh(cube, matrix, skyboxMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
            CommandBufferPool.Release(cmd);
        }
    }

}

