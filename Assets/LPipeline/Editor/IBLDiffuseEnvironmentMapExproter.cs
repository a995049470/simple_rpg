using System.Collections;
using System.Collections.Generic;
using NullFramework.Editor;
using NullFramework.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace LPipeline.Editor
{

    [CreateAssetMenu(fileName = "IBLDiffuseEnvironmentMapExproter", menuName = "SimpleWindow/IBLDiffuseEnvironmentMapExproter")]
    public class IBLDiffuseEnvironmentMapExproter : SimpleWindow
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
            var cube = MeshUitily.CreateCube();
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
            var path = $"{absOutFloder}/{originEnvMap.name}_diffuse.png";
            System.IO.File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.Refresh();
            var importer = AssetImporter.GetAtPath(FileUtility.AbsPathToLocalPath(path)) as TextureImporter;
            importer.textureShape = TextureImporterShape.TextureCube;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
        #endregion
        
    }
    

}
