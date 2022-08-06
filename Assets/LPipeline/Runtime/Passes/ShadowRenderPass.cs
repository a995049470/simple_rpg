using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{

    [CreateAssetMenu(fileName = "ShadowRenderPass", menuName = "LPipeline/Passes/ShadowRenderPass")]
    public class ShadowRenderPass : RenderPass
    {
        [SerializeField]
        private Vector2Int shadowMapResolution = new Vector2Int(1024, 1024);
        public Vector2Int ShadowMapResolution 
        {
            get
            {
                var x = Mathf.Max(1, shadowMapResolution.x);
                var y = Mathf.Max(1, shadowMapResolution.y);
                return new Vector2Int(x, y);
            }
        }
        [SerializeField]
        private float width = 256;
        public float Width { get => Mathf.Max(1, width); }
        [SerializeField]
        private float height = 256;
        public float Height { get => Mathf.Max(1, height); }
        [SerializeField]
        private float near = 0;
        public float Near { get => Mathf.Max(0, near); }
        [SerializeField]
        private float far = 64;
        public float Far { get => Mathf.Max(Near + 1, far); }

        private RenderTargetHandle shadowMap;
        
        private List<ShaderTagId> shaderTagIdList;


        public override void FirstCall() {
            shadowMap.Init("_ShadowMap");
            shaderTagIdList = new List<ShaderTagId>()
            {
                new ShaderTagId("ShadowCaster"),
            };
        }
        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            
            
            var shadowLight = LightManager.Instance.GetShadowLight();
            if(shadowLight == null)
            {
                return;
            }

            var cmd = CommandBufferPool.Get(nameof(ShadowRenderPass));
            var des = new RenderTextureDescriptor(ShadowMapResolution.x, ShadowMapResolution.y, RenderTextureFormat.Depth);
            des.depthBufferBits = 32;
            des.sRGB = false;
            cmd.GetTemporaryRT(shadowMap.id, des, FilterMode.Point);
            cmd.SetRenderTarget(shadowMap.Identifier(), shadowMap.Identifier());
            cmd.ClearRenderTarget(true, true, Color.black);

            var to = GetLookTarget(data.camera);
            var from = GetStartPoint(to, shadowLight.transform.forward);
            //var view = Matrix4x4.LookAt(from, to, Vector3.up);
             var view = Matrix4x4.TRS(from, shadowLight.transform.rotation, Vector3.one);
            view = view.inverse;
            for (int i = 0; i < 4; i++)
            {
                view[2, i] *= -1;
            }
            var projection = Matrix4x4.Ortho(-width * 0.5f, width * 0.5f, -height * 0.5f, height * 0.5f, near, far);
            projection = GL.GetGPUProjectionMatrix(projection, true);
            var vp = projection * view;
            cmd.SetGlobalMatrix(ShaderUtils._ShadowVP, vp);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            //开始渲染shadowMap
            var filteringSettings = FilteringSettings.defaultValue;
            var drawingSettings = CreateDrawingSettings(shaderTagIdList, data, SortingCriteria.CommonOpaque);

            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref filteringSettings);
            
            CommandBufferPool.Release(cmd);
        }
        
        private Vector3 GetStartPoint(Vector3 to, Vector3 forward)
        {
            return to - (Far - Near) * 0.5f * forward;
        }

        private Vector3 GetLookTarget(Camera camera)
        {
            //用相机z轴方向与零平面的交点作为目标点
            var dir = camera.transform.forward;
            if(Mathf.Abs(dir.y) < 0.0001f)
            {
                dir.y = (dir.y > 0 ? 1 : -1) * 0.0001f;
            }

            var y = -camera.transform.position.y / dir.y;
            var landPosition = camera.transform.position + y * dir;
            return landPosition;

        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(shadowMap.id);
        }
        
    }

}

