using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleRPG.Runtime;
using UnityEditor;
using System.Text.RegularExpressions;
using NullFramework.Runtime;

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
            if(GUILayout.Button("拷贝一份数据"))
            {
                Copy(leafMono);
            }

            if(GUILayout.Button("删除物体和数据"))
            {
                Delete(leafMono);
            }
        }

        public void Copy(LeafMono leaf)
        {
            var data = leaf?.Data;
            if(data == null) return;
            var path = AssetDatabase.GetAssetPath(data);
            var type = data.GetType().Name;
            var floder = FileUtility.GetNearestFloder(path);
            var absFloder = FileUtility.LocalPathToAbsPath(floder);
            var files = System.IO.Directory.GetFiles(absFloder, "*.asset", System.IO.SearchOption.TopDirectoryOnly);
            var max = 0;
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                if(!Regex.IsMatch(file,  $"{type}.*[.]asset"))continue;
                var match = Regex.Match(file, "[0-9]+[.]");
                if(!string.IsNullOrEmpty(match.Value))
                {
                    var val = match.Value.Substring(0, match.Value.Length - 1);
                    var num = int.Parse(val) + 1;
                    max = Mathf.Max(num, max);
                }
            }
            var newpath = $"{floder}/{type}_{max}.asset";
            var newData = Instantiate(data);
            AssetDatabase.CreateAsset(newData, newpath);
            leaf.SetData(newData);
        }

        public void Delete(LeafMono leaf)
        {
            var data = leaf?.Data;
            if(data != null)
            {
                var path = AssetDatabase.GetAssetPath(leaf.Data);
                AssetDatabase.DeleteAsset(path);
            }
            DestroyImmediate(leaf.gameObject, true);
            
        }
        
    }
}

