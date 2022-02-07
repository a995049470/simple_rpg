using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;
using System.IO;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    public static class SerializationHelper 
    {
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

        public static T GetValue<T>(string file, string key)
        {
            LitJsonUtility.LitJsonExpand();
            var data = LoadJsonData(file);
            if(!data.ContainsKey(key))
            {
                return default;
            }
            var value = data[key];
            var type = typeof(T);
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
            else
            {
                res = JsonMapper.ToObject<T>(value.ToJson());
            }
            return (T)res;
            
        }

    }

}

