using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LPipeline.Runtime
{

    [CreateAssetMenu(fileName = "Render2DCellLightPass", menuName = "LPipeline/Passes/Render2DCellLightPass")]
    public class Render2DCellLightPass : RenderPass
    {
        public class CellLightTexture
        {
            private RenderTexture rt;
            public RenderTexture RT { get => rt; } 
            private Rect area;
            public Rect Area { get => area; }
            private Rect safeArea;
            private int perCellSize;
            
            public CellLightTexture(int _perCellSize)
            {
                perCellSize = _perCellSize;
            }

            private bool IsInSafeArea(Rect rect)
            {
                return safeArea.Contains(new Vector2(rect.xMin, rect.yMin)) && safeArea.Contains(new Vector2(rect.xMax, rect.yMax));
            }

            private bool IsSameRTSize(int width, int height)
            {
                return rt != null && rt.width == width && rt.height == height;
            }

            public void Set(Rect worldRect, Material copyMaterial, CommandBuffer cmd, ScriptableRenderContext context)
            {
                var areaWidth = NearestPowOfTwo(worldRect.width * 2);
                var areaHeight = NearestPowOfTwo(worldRect.width * 2);

                var rtWidth = NearestPowOfTwo(perCellSize * worldRect.width * 2);
                var rtHeight = NearestPowOfTwo(perCellSize * worldRect.height * 2);

                var isInSafeArea = IsInSafeArea(worldRect);
                var isSameRTSize = IsSameRTSize(rtWidth, rtHeight);
                
                //remap
                if(!isInSafeArea || !isSameRTSize)
                {
                    var blockWidth = areaWidth / 4.0f;
                    var blockHeight = areaHeight / 4.0f;
                    var areaStartPoint = new Vector2
                    (
                        Mathf.Floor(worldRect.x / blockWidth - 1) * blockWidth,
                        Mathf.Floor(worldRect.y / blockHeight - 1) * blockHeight
                    );
                    var lastArea = area;
                    area = new Rect(areaStartPoint.x, areaStartPoint.y, areaWidth, areaHeight);
                    safeArea = new Rect(areaStartPoint.x + blockWidth * 0.5f, areaStartPoint.y + blockHeight * 0.5f, 3 * blockWidth, 3 * blockHeight);
                    var des = new RenderTextureDescriptor(rtWidth, rtHeight, RenderTextureFormat.ARGBHalf);
                    var temp = RenderTexture.GetTemporary(des);
                    temp.filterMode = FilterMode.Bilinear;
                    temp.Create();
                    if(rt != null)
                    {
                        cmd.SetGlobalVector(ShaderUtils._CellLightWorldArea, new Vector4(area.xMin, area.xMax, area.yMin, area.yMax));
                        cmd.SetGlobalVector(ShaderUtils._LastCellLightWorldArea, new Vector4(lastArea.xMin, lastArea.xMax, lastArea.yMin, lastArea.yMax));
                        cmd.Blit(rt, temp, copyMaterial);
                    }
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    if(rt != null) RenderTexture.ReleaseTemporary(rt);
                    rt = temp;
                }   

            }

        }

        [SerializeField]
        private int perCellSize = 4;
        [SerializeField]
        private Material copyMaterial;
        private Dictionary<CameraType, CellLightTexture> cellLightTextureDic = new Dictionary<CameraType, CellLightTexture>();
        private List<ShaderTagId> cellTags;
        private RenderTargetHandle cellTexture;



        public override void FirstCall()
        {
            base.FirstCall();
            cellTags = new List<ShaderTagId>()
            {
                new ShaderTagId("Cell"),
            };
            cellTexture.Init("_CellTexture");
        }

        private bool TryGetCurrentCellLightTexture(ScriptableRenderContext context, CommandBuffer cmd, RenderData data, out CellLightTexture cellLightTex)
        {
            var camera = data.camera;
            var cameraType = camera.cameraType;
            cellLightTex = null;
            if(!camera.orthographic) return false;
            var ratio = (float)camera.pixelWidth / camera.pixelHeight;
            var halfHeight = camera.orthographicSize;
            var halfWidth = halfHeight * ratio;
            var cameraPosition = camera.transform.position;
            var leftDownPoint = new Vector2(cameraPosition.x - halfWidth, cameraPosition.y - halfHeight);
            var worldRect = new Rect(leftDownPoint.x, leftDownPoint.y, 2 * halfWidth, 2 * halfHeight);
            if(!cellLightTextureDic.TryGetValue(cameraType, out cellLightTex))
            {
                cellLightTex = new CellLightTexture(perCellSize);
                cellLightTextureDic[cameraType] = cellLightTex;
            }
            cellLightTex.Set(worldRect, copyMaterial, cmd, context);
            return true;
        }

        private static int NearestPowOfTwo(float value)
        {
            return 1 << Mathf.CeilToInt(Mathf.Log(value, 2));
        }

        private bool IsCanExecute()
        {
            return copyMaterial != null;
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {   
            if(!IsCanExecute())
            {
                throw new System.Exception("无法运行,缺少序列化数据");
            }
            var cmd = CommandBufferPool.Get(nameof(Render2DCellLightPass));
            if(TryGetCurrentCellLightTexture(context, cmd, data, out var cellLightTex))
            {
                //开始渲染cell
                var area = cellLightTex.Area;
                cmd.SetGlobalVector(ShaderUtils._CellLightWorldArea, new Vector4(area.xMin, area.xMax, area.yMin, area.yMax));
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                var drawSetting = CreateDrawingSettings(cellTags, data, SortingCriteria.None);
                
                
            }
            CommandBufferPool.Release(cmd);
        }
    }
}

