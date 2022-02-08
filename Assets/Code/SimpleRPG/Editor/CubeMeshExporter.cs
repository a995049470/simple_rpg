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
        private Mesh cube;
        [SerializeField]
        private float cuebsize;
        
    }
}

