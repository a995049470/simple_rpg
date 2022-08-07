using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using NullFramework.Runtime;
using UnityEditor.SceneManagement;

namespace NullFramework.Editor
{

    public static class EditorHelper
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
                if (value != null) list.Add(value);
            }
            return list;
        }

        [MenuItem("GameObject/保存属性到预制体", false, 0)]
        private static void SaveAsPrefab()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject go)
                {
                    var root = PrefabUtility.GetNearestPrefabInstanceRoot(go);
                    var asset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(root) as GameObject;
                    if (asset == null) continue;
                    asset.transform.localPosition = root.transform.localPosition;
                    asset.transform.localRotation = root.transform.localRotation;
                    asset.transform.localScale = root.transform.localScale;
                    PrefabUtility.SavePrefabAsset(asset);
                    PrefabUtility.ApplyPrefabInstance(root, InteractionMode.AutomatedAction);

                }
            }
            AssetDatabase.Refresh();

        }

        [MenuItem("Tool/ChangeAcitveState %w", false, 5)]
        private static void ActiveGameObject()
        {
            foreach (var go in Selection.gameObjects)
            {
                go.SetActive(!go.activeSelf);
            }
            if (!UnityEngine.Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}

