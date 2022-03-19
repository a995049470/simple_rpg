using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    public abstract class RenderPass : ScriptableObject
    {
        private static Mesh fullScreenQuad;
        private static RenderTargetHandle tempRT;
        public static RenderTargetHandle TempRT
        {
            get
            {
                if(tempRT.id == -1)
                {
                    tempRT.Init("_TempRT");
                }
                return tempRT;
            }
        }

        [SerializeField]
        private bool enable = true;
        public bool Enable { get => enable; }
        [SerializeField]
        private CameraType targetCameraType = (CameraType)31;
        public CameraType TargetCameraType { get => targetCameraType; }

        public abstract void Execute(ScriptableRenderContext context, RenderData data);

        public virtual void FrameCleanup(CommandBuffer cmd)
        {

        }


        //使用默认的颜色和深度缓冲作为渲染对象
        protected void SetDefaultRenderTarget(CommandBuffer cmd, ScriptableRenderContext context, RenderData data)
        {
            cmd.SetRenderTarget(data.activeCameraColorAttachment, data.activeCameraDepthAttachment);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        protected DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTagIdList, RenderData data, SortingCriteria sortingCriteria)
        {
            if(shaderTagIdList == null || shaderTagIdList.Count == 0)
            {
                throw new System.Exception("至少包括一个shaderTagId");
            }
            var shaderTagId = shaderTagIdList[0];
            Camera camera = data.camera;
            SortingSettings sortingSettings = new SortingSettings(camera);
            sortingSettings.criteria = sortingCriteria;

            DrawingSettings settings = new DrawingSettings(shaderTagId, sortingSettings);
            settings.enableDynamicBatching = data.enableDynamicBatching;
            for (int i = 1; i < shaderTagIdList.Count; ++i)
                settings.SetShaderPassName(i, shaderTagIdList[i]);
            return settings;
        }

        protected Mesh GetFullScreenQuad()
        {
            if(fullScreenQuad == null)
            {
                fullScreenQuad = new Mesh();
                fullScreenQuad.vertices = new Vector3[]
                {
                    new Vector3(-1, -1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(-1, 1, 0)
                };
                fullScreenQuad.uv = new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                };
                fullScreenQuad.SetIndices(new int[]
                {
                    0, 2, 1,
                    0, 3, 2,
                }, MeshTopology.Triangles, 0);
            }
            return fullScreenQuad;
        }
    }

}

