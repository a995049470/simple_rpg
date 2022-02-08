using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NullFramework.Editor;

namespace SimpleRPG.Runtime
{
    public class CubeMeshExporter : JsonWindow<CubeMeshExporter>
    {
        [SerializeField]
        private Texture2D texture;
        [SerializeField]
        private Material material;
        [SerializeField]
        private Mesh cubeMesh;
        [SerializeField]
        private float cuebsize;
        [SerializeField]
        private string exportFloder;
        [SerializeField]
        private float alphaThreshold;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;

        private void TextureToCube()
        {
            var go = new GameObject(texture.name);
            for (int i = 0; i < width; i++) 
            {
                for (int j = 0; j < height; j++)
                {
                    // var cube = new GameObject($"cube_{i}_{j}");
                    // cube.AddComponent
                }
            }
        }
        
        
    }
}

