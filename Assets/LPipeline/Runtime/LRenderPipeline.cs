using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    public class RenderData
    {
        public Camera camera;
        public CullingResults cullingResults;
        public Vector2Int renderTextureSize;
        public int renderWidth;
        public int renderHeight;
        public RenderTargetIdentifier activeCameraColorAttachment;
        public RenderTargetIdentifier activeCameraDepthAttachment;
        public RenderTextureDescriptor colorDescriptor;
        public bool enableDynamicBatching;
        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 gpuProjectionMatrix;
    }

    

    public class LRenderPipeline : RenderPipeline
    {

        RenderTargetHandle cameraColorAttachment;
        //RenderTargetHandle cameraDepthAttachment;
        LPipelineRenderSetting pipelineRenderSetting;

        public LRenderPipeline(LPipelineRenderSetting setting)
        {
            cameraColorAttachment.Init("_CameraColorTexture");
            //cameraDepthAttachment.Init("_CameraDepthAttachment");
            this.pipelineRenderSetting = setting;
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context, cameras);

            //摄像机排序的必要性??
            
            //渲染每个摄像机
            for (int i = 0; i < cameras.Length; i++)
            {
                var camera = cameras[i];
                BeginCameraRendering(context, camera);
                RenderSingleCamera(context, camera);
                EndCameraRendering(context, camera);
            }

            EndFrameRendering(context, cameras);
        }
        
        //渲染顺序
        //1.创建一个颜色缓存和深度模板缓存
        private void RenderSingleCamera(ScriptableRenderContext context, Camera camera)
        {
            //将相机的属性提交
            context.SetupCameraProperties(camera);

            //给renderData赋值
            RenderData data = new RenderData();
            data.camera = camera;
            if(camera.TryGetCullingParameters(out var cullingParameters))
            {
                var cullingResults = context.Cull(ref cullingParameters);
                data.cullingResults = cullingResults;
            }
            data.renderWidth = camera.pixelWidth;
            data.renderHeight = camera.pixelHeight;
            data.enableDynamicBatching = pipelineRenderSetting.enableDynamicBatching;
            data.viewMatrix = camera.worldToCameraMatrix;
            data.projectionMatrix = camera.projectionMatrix;
            data.gpuProjectionMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true);
            
            //生成颜色缓冲和深度模板缓冲并清理
            var cmd = CommandBufferPool.Get("Render Camera");
            var colorDes = new RenderTextureDescriptor(data.renderWidth, data.renderHeight, RenderTextureFormat.RGB111110Float);
            colorDes.graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            var depthDes = new RenderTextureDescriptor(data.renderWidth, data.renderHeight, RenderTextureFormat.Depth);
            depthDes.depthBufferBits = 32;
            depthDes.msaaSamples = 1;

            cmd.GetTemporaryRT(cameraColorAttachment.id, colorDes);
            //cmd.GetTemporaryRT(cameraDepthAttachment.id, depthDes, FilterMode.Point);
            //清理
            cmd.SetRenderTarget(cameraColorAttachment.Identifier());
            cmd.ClearRenderTarget(true, true, Color.black);
            

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            //赋值data
            data.activeCameraColorAttachment = cameraColorAttachment.Identifier();
            data.activeCameraDepthAttachment = cameraColorAttachment.Identifier();
            data.colorDescriptor = colorDes;

            //开始运行运行各个pass
            var passes = pipelineRenderSetting.passes;
            int passCount = passes?.Count ?? 0;
            for (int i = 0; i < passCount; i++)
            {
                var pass = passes[i];
                if(pass == null || !pass.Enable || ((int)pass.TargetCameraType & (int)camera.cameraType) == 0)
                {
                    continue;
                }
                pass.Execute(context, data);
            }
            
            //将颜色blit到camera的RT上
            var targetRT = camera.targetTexture;
            cmd.Blit(cameraColorAttachment.Identifier(), targetRT);
            

            //开始清理
            cmd.ReleaseTemporaryRT(cameraColorAttachment.id);
            //cmd.ReleaseTemporaryRT(cameraDepthAttachment.id);
            for (int i = 0; i < passCount; i++)
            {
                passes[i]?.FrameCleanup(cmd);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);

            //给场景绘制Gizmos
            DrawGizmos(context, camera);

            //提交上下文
            context.Submit();
            
        }

        

        private void DrawGizmos(ScriptableRenderContext context, Camera camera) {
    #if UNITY_EDITOR
        if(camera.cameraType != CameraType.SceneView)
        {
            return;
        }
		if (UnityEditor.Handles.ShouldRenderGizmos()) {
			context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
			context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
		}
    #endif
	}
        
        
        

    }

}

