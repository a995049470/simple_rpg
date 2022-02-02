
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{

    [CreateAssetMenu(fileName = "DrawObjectPass", menuName = "LPipeline/Passes/DrawObjectPass")]
    public class DrawObjectPass : RenderPass
    {
        
        //序列化部分
        [SerializeField]
        private string[] shaderTags;
        [SerializeField]
        private string profilerTag = nameof(DrawObjectPass);
        [SerializeField]
        private bool isOpaque = true;  

        private List<ShaderTagId> shaderTagIdList;
        private FilteringSettings filteringSettings;

        public void OnEnable()
        {
            Init();
        }

   

        private void Init()
        {
            int count = shaderTags?.Length ?? 0;
            shaderTagIdList = new List<ShaderTagId>();
            for (int i = 0; i < count; i++)
            {
                shaderTagIdList.Add(new ShaderTagId(shaderTags[i]));
            }
            filteringSettings = new FilteringSettings(isOpaque ? RenderQueueRange.opaque : RenderQueueRange.transparent);
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            base.Execute(context, data);
            var cmd = CommandBufferPool.Get(profilerTag);
            SetDefaultRenderTarget(cmd, context, data);
            var sortFlags = isOpaque ? SortingCriteria.CommonOpaque : SortingCriteria.CommonTransparent;
            var drawSetting  = CreateDrawingSettings(shaderTagIdList, data, sortFlags);
            context.DrawRenderers(data.cullingResults, ref drawSetting, ref filteringSettings);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        //必须保证shaderTagIdList数量大于0
        


        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
        }
        
        private void OnValidate() {
            Init();
        }
       

    }

}

