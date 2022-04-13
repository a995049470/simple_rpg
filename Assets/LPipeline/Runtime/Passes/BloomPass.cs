using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "BloomPass", menuName = "LPipeline/Passes/BloomPass")]
    public class BloomPass : RenderPass
    {
        //pass 0:copy 1:down 2:up 3:add
        [SerializeField]
        private Material bloomMaterial;
        [SerializeField]
        [Range(1, 15)]
        private int iteratorCount;
        private const int maxLevel = 16;
        private const int maxIteratorCount = maxLevel - 1;
        private RenderTargetHandle[] mipmaps = new RenderTargetHandle[maxLevel];
        private const int filterPass = 0;
        private const int downsamplePass = 1;
        private const int upsmaplePass = 2;
        private const int applyBloomPass = 3;


        private void OnEnable() {
            for (int i = 0; i < maxLevel; i++)
            {
                mipmaps[i].Init($"_BlurMipMap{i}");
            }
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
        #if UNITY_EDITOR
            if(iteratorCount < 0 || iteratorCount > maxIteratorCount)
            {
                throw new System.Exception("迭代次数不合法");
            }
            if(bloomMaterial == null)
            {
                throw new System.Exception("没材质 跑不了");
            }
        #endif
            var cmd = CommandBufferPool.Get(nameof(BloomPass));
            var des = data.colorDescriptor;
            for (int i = 0; i < iteratorCount + 1; i++)
            {
                cmd.GetTemporaryRT(mipmaps[i].id, des);    
                des.width = Mathf.Max(des.width / 2, 1);
                des.height = Mathf.Max(des.height / 2, 1);
            }
            var lastDown = mipmaps[0];
            //filter
            cmd.Blit(data.activeCameraColorAttachment, lastDown.Identifier(), bloomMaterial, filterPass);
            //downsample
            for (int i = 1; i < iteratorCount + 1; i++)
            {
                var down = mipmaps[i];
                cmd.Blit(lastDown.Identifier(), down.Identifier(), bloomMaterial, downsamplePass);
                lastDown = down;
            }
            var lastUp = lastDown;
            //upsample
            for (int i = iteratorCount - 1; i >= 0; i--)
            {
                var up = mipmaps[i];
                cmd.Blit(lastUp.Identifier(), up.Identifier(), bloomMaterial, upsmaplePass);
                lastUp = up;
            }
            //applyBloom
            cmd.Blit(lastUp.Identifier(), data.activeCameraColorAttachment, bloomMaterial, applyBloomPass);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            for (int i = 0; i < iteratorCount + 1; i++)
            {
                cmd.ReleaseTemporaryRT(mipmaps[i].id);
            }
        }
    }

}

