using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "Render2DLightPass", menuName = "LPipeline/Passes/Render2DLightPass")]
    public class Render2DLightPass : RenderPass
    {
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            throw new System.NotImplementedException();
        }
    }

}

