using System.IO;
using NullFramework.Editor;
using NullFramework.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LPipeline.Editor
{
    [CreateAssetMenu(fileName = "BRDFPrecomputeMapExporter", menuName = "SimpleWindow/BRDFPrecomputeMapExporter")]
    public class BRDFPrecomputeMapExporter : SimpleWindow
    {   
        [SerializeField] 
        private int width;
        [FolderPath]
        [SerializeField]
        private string outFloder;
        [SerializeField]
        private Material precompteMapMaterial;
        
        
        [Button("导出BRDF预计算图")]
        private void Exporte()
        {
            var height = width;
            var des = new RenderTextureDescriptor(width, height, RenderTextureFormat.RGHalf);
            des.sRGB = false;
            var rt = new RenderTexture(des);
            Graphics.SetRenderTarget(rt);
            Graphics.Blit(Texture2D.blackTexture, rt, precompteMapMaterial);
            var tex = new Texture2D(width, height, TextureFormat.RGHalf, 0, true);
            var active = RenderTexture.active;
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = active;

            // var absOutFloder = FileUtility.LocalPathToAbsPath(outFloder);
            // var absPath = $"{absOutFloder}/lut_brdf.png";
            // var bytes = tex.EncodeToPNG();
            // File.WriteAllBytes(absPath, bytes);
            
            var path = $"{outFloder}/lut_brdf.asset";
            UnityEditor.AssetDatabase.CreateAsset(tex, path);
            
           
        }
    }
    

}
