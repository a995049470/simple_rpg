using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    [System.Serializable]
    public class LPipelineRenderSetting
    {
        [SerializeField]
        public List<RenderPass> passes;
        public bool enableDynamicBatching = false;
        [SerializeField]
        private Color editorBackgroundColor = Color.black;
        public Color EditorBackgroundColor
        {
            get
            {
                var r = editorBackgroundColor.r;
                var g = editorBackgroundColor.g;
                var b = editorBackgroundColor.b;
                return new Color(r * r, g * g, b * b);
                
            }
        }
    }

    [CreateAssetMenu(fileName = "CustomRenderPipline", menuName = "LPipeline/PipelineAsset")]
    public class LRenderPipelineAsset : RenderPipelineAsset
    {
        [SerializeField]
        LPipelineRenderSetting setting;
        public LPipelineRenderSetting Setting { get => setting;}
        protected override RenderPipeline CreatePipeline()
        {
            return new LRenderPipeline(setting);
        }

    }
}

