using System.Collections.Generic;
using System.Reflection;

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

}

