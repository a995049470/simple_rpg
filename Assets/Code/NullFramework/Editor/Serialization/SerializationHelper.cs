using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;
using System.IO;
using NullFramework.Runtime;
using System.Reflection;
using System;

namespace NullFramework.Editor
{
    //序列化数据会存在一个Json文件里
    public abstract class JsonWindow
    {
        private List<FieldInfo> cacheFields;
        public void LoadData(string cacheFile = null)
        {
            if(cacheFields == null)
            {
                cacheFields = new List<FieldInfo>();
                var type = this.GetType();
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var fields = type.GetFields(flags);
                foreach (var field in fields)
                {
                    if(field.IsPublic || field.GetCustomAttribute<UnityEngine.SerializeField>(false) != null)
                    {
                        cacheFields.Add(field);
                    }
                }
            }
            SerializationHelper.ClearCache();
            var file = string.IsNullOrEmpty(cacheFile) ? this.GetType().Name : cacheFile;
            foreach (var field in cacheFields)
            {
                var value = SerializationHelper.GetValue(file, field.Name, field.FieldType);
                field.SetValue(this, value);
            }
        }
    }

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
                return default;
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

