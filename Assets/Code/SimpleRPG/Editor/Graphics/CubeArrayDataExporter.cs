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
        private Texture2D texture;
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
            Instance.TextureToCube();
            AssetDatabase.Refresh();
            CubeRendererCenter.Instance.Refresh();
        }

        private void TextureToCube()
        {
            //转换成适合大小的texture
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGBFloat);
            var rt = new RenderTexture(des);
            rt.filterMode = FilterMode.Point;
            rt.Create();
            Graphics.Blit(texture, rt);
            var suitTexture = new Texture2D(width, height);
            var active = RenderTexture.active;
            RenderTexture.active = rt;
            suitTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            suitTexture.Apply();
            rt.Release();
            RenderTexture.active = active;

            var num = width * height;
            var data = new List<CubeGBuffer>();
            for (int i = 0; i < width; i++) 
            {
                var u = (i + 0.5f) * (1.0f / width);
                for (int j = 0; j < height; j++)
                {
                    var v = (j + 0.5f) * (1.0f / height);
                    var pixel = suitTexture.GetPixel(i, j);
                    if(pixel.a < alphaThreshold)
                    {
                        continue;
                    }
                    //float gamma = 2.2f;
                    // pixel.r = Mathf.Pow(pixel.r, gamma);
                    // pixel.g = Mathf.Pow(pixel.g, gamma);
                    // pixel.b = Mathf.Pow(pixel.b, gamma);
                    var id = i + j * width;
                    var pos = new Vector3(step * (i + 0.5f), step * (j + 0.5f), 0);
                    var matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                    var buffer = new CubeGBuffer();
                    buffer.albedo = pixel;
                    buffer.localToWorldMatrix = matrix;
                    buffer.ao = 1;
                    buffer.metallic = 0.5f;
                    buffer.roughness = 0.5f;
                    data.Add(buffer);
                }
            }
            var localPath = $"{exportFloder}/{texture.name}.bytes";
            var absPath = FileUtility.LocalPathToAbsPath(localPath);
            var bytes = StructUtility.ArrayToBytes(data.ToArray());
            File.WriteAllBytes(absPath, bytes);
        }
        
        
    }
}

