using System.Collections;
using System.Collections.Generic;
using NullFramework.Editor;
using NullFramework.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace LPipeline.Editor
{
    [CreateAssetMenu(fileName = "IBLGlossyEnvironmentMapExporter", menuName = "SimpleWindow/IBLGlossyEnvironmentMapExporter")]
    public class IBLGlossyEnvironmentMapExporter : SimpleWindow
    {
        [SerializeField]
        private Cubemap originEnvMap;
        [SerializeField]
        private Material glossyEvnMapBlurMaterial;
        [SerializeField]
        private int unitSize = 128;
        [SerializeField]
        private int minSize = 16;
        [SerializeField]
        [FolderPath]
        private string outFloder;

        [Button("导出镜面辐照图")]
        private void Exporte()
        {
            var height = unitSize * 3;
            var width = unitSize * 4;
            int mipCount = Mathf.RoundToInt(Mathf.Log(unitSize / minSize, 2));
            var cube = MeshUitily.CreateCube();
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0, mipCount);
            des.sRGB = true;
            des.useMipMap = true;
            des.autoGenerateMips = false;
            var rt = RenderTexture.GetTemporary(des);
            rt.filterMode = FilterMode.Trilinear;
            var material = Instantiate(glossyEvnMapBlurMaterial);
            
            for (int i = 0; i < mipCount; i++)
            {
                float roughness =  (float)i / (mipCount - 1);
                Graphics.SetRenderTarget(rt, i);
                material.SetFloat("_Roughness", roughness);
                material.SetTexture("_MainTex", originEnvMap);
                material.SetPass(0);
                Graphics.DrawMeshNow(cube, Matrix4x4.identity, 0);
            }
            
            
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, mipCount, false);
            tex.filterMode = FilterMode.Trilinear;
            var active = RenderTexture.active;
            for (int i = 0; i < mipCount; i++)
            {
                var asyncGPUReadback = AsyncGPUReadback.Request(rt, i);
                asyncGPUReadback.WaitForCompletion();
                tex.SetPixelData(asyncGPUReadback.GetData<byte>(), i);
            }
            tex.Apply(false, false);
            
            RenderTexture.ReleaseTemporary(rt);
            //texture2d to cubemap
            //mipCount = 1;
            var cubemap = new Cubemap(unitSize, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB,UnityEngine.Experimental.Rendering.TextureCreationFlags.MipChain, mipCount);
            
            int unit = unitSize;
            
            
            for (int i = 0; i < mipCount; i++)
            {
                
                var pixels_NX = tex.GetPixels(unit * 0, unit * 1, unit, unit, i);
                var pixels_PX = tex.GetPixels(unit * 2, unit * 1, unit, unit, i);

                var pixels_NY = tex.GetPixels(unit * 1, unit * 0, unit, unit, i);
                var pixels_PY = tex.GetPixels(unit * 1, unit * 2, unit, unit, i);
                
                var pixels_NZ = tex.GetPixels(unit * 3, unit * 1, unit, unit, i);
                var pixels_PZ = tex.GetPixels(unit * 1, unit * 1, unit, unit, i);
                
                ReversePixels(pixels_NX, unit, true, false);
                ReversePixels(pixels_PX, unit, true, false);
                ReversePixels(pixels_NY, unit, true, false);
                ReversePixels(pixels_PY, unit, true, false);
                ReversePixels(pixels_NZ, unit, true, false);
                ReversePixels(pixels_PZ, unit, true, false);
                
                unit /= 2;

                var pixels = tex.GetPixels(i);

                cubemap.SetPixels(pixels_NX, CubemapFace.NegativeX, i);
                cubemap.SetPixels(pixels_PX, CubemapFace.PositiveX, i);
                cubemap.SetPixels(pixels_NY, CubemapFace.NegativeY, i);
                cubemap.SetPixels(pixels_PY, CubemapFace.PositiveY, i);
                cubemap.SetPixels(pixels_NZ, CubemapFace.NegativeZ, i);
                cubemap.SetPixels(pixels_PZ, CubemapFace.PositiveZ, i);
            }
            cubemap.Apply(false, false);
            var path_cubemap = $"{this.outFloder}/{originEnvMap.name}_glossy.asset";
            //AssetDatabase.DeleteAsset(path_cubemap);
            AssetDatabase.CreateAsset(cubemap, path_cubemap);
        }

        void ReversePixels(Color[] pixels, int width, bool isReverseUD, bool isReverseLR)
        {
            if(isReverseUD)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width / 2; j++)
                    {
                        var from = j * width + i;
                        var to = (width - 1 - j) * width + i;
                        var temp = pixels[from];
                        pixels[from] = pixels[to];
                        pixels[to] = temp;
                    }
                }
            }
            if(isReverseLR)
            {
                for (int i = 0; i < width / 2; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        var from = j * width + i;
                        var to = j * width + (width - 1 - i);
                        var temp = pixels[from];
                        pixels[from] = pixels[to];
                        pixels[to] = temp;
                    }
                }

            }
            
        }
    }
    

}
