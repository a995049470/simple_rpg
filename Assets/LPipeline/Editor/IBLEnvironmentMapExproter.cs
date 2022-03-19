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
        #region 导出漫反射辐照图
        [SerializeField]
        private Cubemap originEnvMap;
        [SerializeField]
        [FolderPath]
        private string outFloder;
        [SerializeField]
        private Material IBLBlurMaterial;
        [SerializeField]
        private int unitSize = 32;
        
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
                new Vector2(0.00f, 0.6666f),
                new Vector2(0.25f, 0.6666f),
                new Vector2(0.25f, 0.3333f),
                new Vector2(0.00f, 0.3333f),

                new Vector2(0.25f, 0.6666f),
                new Vector2(0.50f, 0.6666f),
                new Vector2(0.50f, 0.3333f),
                new Vector2(0.25f, 0.3333f),

                new Vector2(0.50f, 0.6666f),
                new Vector2(0.75f, 0.6666f),
                new Vector2(0.75f, 0.3333f),
                new Vector2(0.50f, 0.3333f),

                new Vector2(0.75f, 0.6666f),
                new Vector2(1.00f, 0.6666f),
                new Vector2(1.00f, 0.3333f),
                new Vector2(0.75f, 0.3333f),

                new Vector2(0.25f, 1.0000f),
                new Vector2(0.50f, 1.0000f),
                new Vector2(0.50f, 0.6666f),
                new Vector2(0.25f, 0.6666f),

                new Vector2(0.25f, 0.3333f),
                new Vector2(0.50f, 0.3333f),
                new Vector2(0.50f, 0.0000f),
                new Vector2(0.25f, 0.0000f),
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

        [PropertySpace(10)]
        [Button("导出漫反射辐照图")]
        private void Exproter()
        {
            var width = unitSize * 4;
            var height = unitSize * 3;
            var des = new RenderTextureDescriptor(width, height);
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
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = active;
            var bytes = tex.EncodeToPNG();
            var absOutFloder = FileUtility.LocalPathToAbsPath(outFloder);
            var path = $"{absOutFloder}/{originEnvMap.name}_ibl.png";
            System.IO.File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.CreateAsset(originEnvMap, $"{outFloder}/{originEnvMap.name}.asset");
            UnityEditor.AssetDatabase.Refresh();
        }
        #endregion

    }
    

}
