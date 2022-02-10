using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    public static class AssetDatabaseHelper
    {
        public static List<T> LoadAllAsset<T>(string localFloder, string pattern) where T : Object
        {
            var list = new List<T>();
            var absPath = FileUtility.LocalPathToAbsPath(localFloder);
            var files = Directory.GetFiles(absPath, pattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var path = FileUtility.AbsPathToLocalPath(file);
                var value = AssetDatabase.LoadAssetAtPath<T>(path);
                if(value != null) list.Add(value);
            }
            return list;
        }
    }
}

