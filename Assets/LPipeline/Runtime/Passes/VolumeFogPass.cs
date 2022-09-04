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

        private RenderTargetHandle fogFrontFaceDepthTex;
        private RenderTargetHandle fogBackFaceDepthTex;

        public override void FirstCall()
        {
            base.FirstCall();
            fogFrontFaceDepthTex.Init("_FogFrontFaceDepthTex");
            fogBackFaceDepthTex.Init("_FogBackFaceDepthTex");
        }

        private bool IsCanExecute()
        {
            return volumeFogMaterial != null;
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            if(!IsCanExecute()) return;
            var cmd = CommandBufferPool.Get(nameof(VolumeFogPass));
            var cameraType = data.camera.cameraType;
            if(cameraType == CameraType.SceneView)
            {
                Execute_SceneView(context, data, cmd);
            }
            else
            {

            }

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

