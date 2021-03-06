using System.Collections;
using UnityEngine;
using LitJson;
using UnityEditor;
using System.IO;
using NullFramework.Runtime;
using System;

namespace NullFramework.Editor
{

    public static class SerializationHelper 
    {
        private static string cacheFile;
        private static JsonData cacheData;
    
        private static string GetJsonAssetPath(string file)
        {
            return $"Assets/Settings/{file}.json";
        }

        private static JsonData LoadJsonData(string file)
        {
            var path = GetJsonAssetPath(file);
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if(asset == null)
            {
                return new JsonData();
            } 
            var data = JsonMapper.ToObject(asset.text);
            return data;
        }

        //清理缓存 防止json更改后缓存未变化
        public static void ClearCache()
        {
            cacheFile = null;
        }

        public static object GetValue(string file, string key, Type type)
        {
            LitJsonUtility.LitJsonExpand();
            JsonData data;
            if(cacheFile == file)
            {
                data = cacheData;
            }
            else
            {
                data = LoadJsonData(file);
                cacheFile = file;
                cacheData = data;
            }
            if(!data.ContainsKey(key))
            {
                throw new Exception($"{file}.json miss key : {key}");
            }
            var value = data[key];
            object res = null;
            if(type == typeof(float))
            {
                res = value.ConvertToFloat();
            }
            else if(type == typeof(int))
            {
                res = value.ConvertToInt();
            }
            else if(type == typeof(double))
            {
                res = value.ConvertToDouble();
            }
            else if(type == typeof(string))
            {
                res = value.ConvertToString();
            }
            else if(typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var guid = value.ConvertToString();
                var path = AssetDatabase.GUIDToAssetPath(guid);
                res = AssetDatabase.LoadAssetAtPath(path, type);
            }
            else
            {
                res = JsonMapper.ToObject(value.ToJson(), type);
            }
            return res;
        }

        public static T GetValue<T>(string file, string key)
        {
            
            return (T)GetValue(file, key, typeof(T));
        }

    }

}

