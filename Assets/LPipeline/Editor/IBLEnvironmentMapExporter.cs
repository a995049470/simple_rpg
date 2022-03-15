using System.Collections;
using System.Collections.Generic;
using NullFramework.Editor;
using NullFramework.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LPipeline.Editor
{
    [CreateAssetMenu(fileName = "IBLEnvironmentMapExproter", menuName = "SimpleWindow/IBLEnvironmentMapExproter")]
    public class IBLEnvironmentMapExproter : SimpleWindow
    {
        [SerializeField]
        private Cubemap originEnvMap;
        [SerializeField]
        private string outFloder;
        [SerializeField]
        private Material IBLBlurMaterial;
        
        private Mesh CreateCube()
        {
            var mesh = new Mesh();
            var vertices = new Vector3[]
            {
                //   5
                // 1 2 3 4
                //   6 
                new Vector3(-1, 1, -1),
                new Vector3(-1, 1, 1),
                new Vector3(-1, -1, 1),
                new Vector3(-1, -1, -1),

                new Vector3(-1, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, -1, 1),
                new Vector3(-1, -1, 1),

                new Vector3(1, 1, 1),
                new Vector3(1, 1, -1),
                new Vector3(1, -1, -1),
                new Vector3(1, -1, 1),

                new Vector3(1, 1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, -1),
                new Vector3(1, -1, -1),
                
                new Vector3(-1, 1, -1),
                new Vector3(1, 1, -1),
                new Vector3(1, 1, 1),
                new Vector3(-1, 1, 1),

                new Vector3(-1, -1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, -1, -1),
            };
            var uv = new Vector2[]
            {
                new Vector2(0.00f, 0.66f),
                new Vector2(0.25f, 0.66f),
                new Vector2(0.25f, 0.33f),
                new Vector2(0.00f, 0.33f),

                new Vector2(0.25f, 0.66f),
                new Vector2(0.50f, 0.66f),
                new Vector2(0.50f, 0.33f),
                new Vector2(0.25f, 0.33f),

                new Vector2(0.50f, 0.66f),
                new Vector2(0.75f, 0.66f),
                new Vector2(0.75f, 0.33f),
                new Vector2(0.50f, 0.33f),

                new Vector2(0.75f, 0.66f),
                new Vector2(1.00f, 0.66f),
                new Vector2(1.00f, 0.33f),
                new Vector2(0.75f, 0.33f),

                new Vector2(0.25f, 0.99f),
                new Vector2(0.50f, 0.99f),
                new Vector2(0.50f, 0.66f),
                new Vector2(0.25f, 0.66f),

                new Vector2(0.25f, 0.33f),
                new Vector2(0.50f, 0.33f),
                new Vector2(0.50f, 0.00f),
                new Vector2(0.25f, 0.00f),
            };
            var indices = new int[]
            {
                0, 2, 1,
                0, 3, 2,

                4, 6, 5,
                4, 7, 6,

                8, 10, 9,
                8, 11, 10,

                12, 14, 13,
                12, 15, 14,

                16, 18, 17,
                16, 19, 18,

                20, 22, 21,
                20, 23, 22,
            };
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            return mesh;
        }

        [Button("导出")]
        private void Exproter()
        {
            var widht = originEnvMap.width * 4;
            var height = originEnvMap.height * 3;
            var des = new RenderTextureDescriptor(widht, height);
            des.sRGB = true;
            var rt = new RenderTexture(des);
            Graphics.SetRenderTarget(rt);
            var cube = CreateCube();
            var mat = Object.Instantiate(IBLBlurMaterial);
            mat.SetTexture("_MainTex", originEnvMap);
            mat.SetPass(0);
            Graphics.DrawMeshNow(cube, Matrix4x4.identity, 0);

            var active = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(widht, height, TextureFormat.RGBA32, false, false);
            tex.ReadPixels(new Rect(0, 0, widht, height), 0, 0);
            tex.Apply();
            
            var bytes = tex.EncodeToPNG();
            var absOutFloder = FileUtility.LocalPathToAbsPath(outFloder);
            var path = $"{absOutFloder}/{originEnvMap.name}_ibl.png";
            System.IO.File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.CreateAsset(tex, $"{outFloder}/{originEnvMap.name}.asset");
            UnityEditor.AssetDatabase.Refresh();

        }
    }

}
