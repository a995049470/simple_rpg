using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{
    [CreateAssetMenu(fileName = "VolumeFogPass", menuName = "LPipeline/Passes/VolumeFogPass")]
    public class VolumeFogPass : RenderPass
    {
        //0:场景摄像机下标记云
        //1.前深度的渲染
        //2.后深度的渲染
        //3.渲染云
        [SerializeField]
        private Material volumeFogMaterial;
        [SerializeField]
        private int scnenViewPass = 0;
        [SerializeField]
        private int frontDepthPass = 1;
        [SerializeField]
        private int backDepthPass = 2;
        [SerializeField]
        private int fogRenderPass = 3;

        private RenderTargetHandle fogFrontFaceDepthTex;
        private RenderTargetHandle fogBackFaceDepthTex;

        public override void FirstCall()
        {
            base.FirstCall();
            fogFrontFaceDepthTex.Init("_FogFrontFaceDepthTex");
            fogBackFaceDepthTex.Init("_FogBackFaceDepthTex");
        }

        private bool IsCanExecute(CameraType cameraType)
        {
            int max = scnenViewPass;
            if(cameraType != CameraType.SceneView)
            {
                max = Mathf.Max(frontDepthPass, backDepthPass, fogRenderPass);
            }
            return volumeFogMaterial != null && volumeFogMaterial.passCount > max;
        }

       

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            var cameraType = data.camera.cameraType;
            if(!IsCanExecute(cameraType)) return;
            var cmd = CommandBufferPool.Get(nameof(VolumeFogPass));
            if(cameraType == CameraType.SceneView)
            {
                Execute_SceneView(context, data, cmd);
            }
            else
            {
                Execute_Game(context, data, cmd);
            }

            CommandBufferPool.Release(cmd);

        }

        private void Execute_Game(ScriptableRenderContext context, RenderData data, CommandBuffer cmd)
        {
            var width = data.renderWidth;
            var height = data.renderHeight;
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.Depth);
            cmd.GetTemporaryRT(fogFrontFaceDepthTex.id, des);
            cmd.GetTemporaryRT(fogBackFaceDepthTex.id, des);
            var fogsDic = VolumeFogManager.Instance.FogsDic;
            foreach (var fogs in fogsDic.Values)
            {
                //渲染正面
                cmd.SetRenderTarget(fogFrontFaceDepthTex.Identifier());
                cmd.ClearRenderTarget(true, false, Color.black, 1.0f);
                foreach (var fog in fogs)
                {   
                    cmd.DrawMesh(fog.FogMesh, fog.transform.localToWorldMatrix, volumeFogMaterial, 0, frontDepthPass);
                }

                //渲染背面
                cmd.SetRenderTarget(fogBackFaceDepthTex.Identifier());
                cmd.ClearRenderTarget(true, false, Color.black, 1.0f);
                foreach (var fog in fogs)
                {   
                    cmd.DrawMesh(fog.FogMesh, fog.transform.localToWorldMatrix, volumeFogMaterial, 0, frontDepthPass);
                }
                
                //用背面作为ray march的起点
                cmd.SetRenderTarget(data.activeCameraColorAttachment, fogBackFaceDepthTex.Identifier());
                foreach (var fog in fogs)
                {
                    cmd.DrawMesh(fog.FogMesh, fog.transform.localToWorldMatrix, volumeFogMaterial, 0, fogRenderPass);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

        }

        private void Execute_SceneView(ScriptableRenderContext context, RenderData data, CommandBuffer cmd)
        {
            var fogsDic = VolumeFogManager.Instance.FogsDic;
            SetDefaultRenderTarget(cmd, context, data);
            foreach (var fogs in fogsDic.Values)
            {
                foreach (var fog in fogs)
                {
                    if(!fog.IsVaild()) continue;
                        cmd.DrawMesh(fog.FogMesh, fog.transform.localToWorldMatrix, volumeFogMaterial, 0, scnenViewPass);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }

}

