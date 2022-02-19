using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleRPG.Runtime;
using UnityEditor;

namespace SimpleRPG.Editor
{
    [CustomEditor(typeof(LeafMono))]
    public class CustomEditor_LeafMono : UnityEditor.Editor
    {
        // We'll cache the editor here
        private UnityEditor.Editor cachedEditor;
        private ScriptableObject cacheData;

        public void OnEnable()
        {
            cachedEditor = null;
            cacheData = null;
        }
        
        public override void OnInspectorGUI()
        {
            var leafMono = target as LeafMono;
            var data = leafMono?.Data;
            if (cacheData != data)
            {
                if (data != null)
                {
                    cachedEditor = UnityEditor.Editor.CreateEditor(data);
                }
                else
                {
                    cachedEditor = null;
                }
                cacheData = data;
            }
            base.OnInspectorGUI();
            cachedEditor?.DrawDefaultInspector();

        }
    }
}

