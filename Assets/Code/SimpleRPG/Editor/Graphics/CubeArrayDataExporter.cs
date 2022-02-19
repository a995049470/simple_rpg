using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NullFramework.Editor;
using SimpleRPG.Runtime;
using NullFramework.Runtime;
using System.IO;
using UnityEditor;

namespace SimpleRPG.Editor
{
   
    public class CubeArrayDataExporter : JsonWindow<CubeArrayDataExporter>
    {
        [SerializeField]
        private string sourceFloder;
        [SerializeField]
        private float step;
        [SerializeField]
        private string exportFloder;
        [SerializeField]
        private float alphaThreshold;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;
        
        [MenuItem("Tool/图片转积木")]
        private static void TestTextureToCube()
        {
            Instance.LoadData();
            Instance.ExporterAllCubeArrayData();
            AssetDatabase.Refresh();
            CubeRendererCenter.Instance.Refresh();
        }

        private Texture2D ConvertToFixedSizeTexture(Texture2D _source)
        {
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGBFloat);
            var rt = new RenderTexture(des);
            rt.filterMode = FilterMode.Bilinear;
            rt.Create();
            Graphics.Blit(_source, rt);
            var suitTexture = new Texture2D(width, height);
            var active = RenderTexture.active;
            RenderTexture.active = rt;
            suitTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            suitTexture.Apply();
            rt.Release();
            RenderTexture.active = active;
            return suitTexture;
        }

        private const string albedo = "albedo";
        private const string normal = "normal";
        private const string roughness = "roughness";
        private const string metallic = "metallic";
        private const string ao = "ao";

        private void ExporterAllCubeArrayData()
        {
            var dic = new Dictionary<string, GBufferTextures>();
            var textures = EditorHelper.LoadAllAsset<Texture2D>(sourceFloder, "*.png");
            foreach (var gbufferTexture in textures)
            {
                var res = gbufferTexture.name.Split('_');
                if(res.Length < 2) continue;
                var name = res[0];
                var kind = res[res.Length - 1].ToLower();
                var suitTexture = ConvertToFixedSizeTexture(gbufferTexture);
                if(!dic.ContainsKey(name))
                {
                    var gbuffer = new GBufferTextures();
                    gbuffer.name = name;
                    dic[name] = gbuffer;
                }
                var gbufferTextures = dic[name];
                
                switch (kind)
                {
                    case albedo:
                        gbufferTextures.albedoTexture = suitTexture;
                        break;
                    case normal:
                        gbufferTextures.normalTexture = suitTexture;
                        break;
                    case roughness:
                        gbufferTextures.roughnessTexture = suitTexture;
                        break;
                    case metallic:
                        gbufferTextures.metallicTexture = suitTexture;
                        break;
                    case ao:
                        gbufferTextures.aoTexture = suitTexture;
                        break;
                    default:
                        Debug.Log($"{gbufferTexture.name} 名称非法", gbufferTexture);
                        break;
                }
            }
            
            foreach (var value in dic.Values)
            {
                CreateCubeDataFromGbufferTextures(value);
            }
        }

        private void CreateCubeDataFromGbufferTextures(GBufferTextures gbufferTextures)
        {
            //转换成适合大小的texture
            
            var num = width * height;
            var data = new List<CubeGBuffer>();
            for (int i = 0; i < width; i++) 
            {
                var u = (i + 0.5f) * (1.0f / width);
                for (int j = 0; j < height; j++)
                {
                    var v = (j + 0.5f) * (1.0f / height);
                    var pixel = gbufferTextures.GetAlbedo(i, j);
                    if(pixel.a < alphaThreshold)
                    {
                        continue;
                    }
                    var id = i + j * width;
                    var pos = new Vector3(step * (i + 0.5f), step * (j + 0.5f), 0);
                    var matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                    var buffer = new CubeGBuffer();
                    buffer.albedo = new Vector3(pixel.r, pixel.g, pixel.b);
                    //buffer.albedo = pixel;
                    buffer.localToWorldMatrix = matrix;
                    buffer.normalTS = gbufferTextures.GetNormalTS(i, j);
                    buffer.ao = gbufferTextures.GetAO(i, j);
                    buffer.metallic = gbufferTextures.GetMetallic(i, j);
                    buffer.roughness = gbufferTextures.GetRoughness(i, j);
                    data.Add(buffer);
                }
            }
            var localPath = $"{exportFloder}/{gbufferTextures.name}.bytes";
            var absPath = FileUtility.LocalPathToAbsPath(localPath);
            var bytes = StructUtility.ArrayToBytes(data.ToArray());
            File.WriteAllBytes(absPath, bytes);
        }
        
        
    }
}

