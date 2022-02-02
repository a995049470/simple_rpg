using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

using System.IO;
using UnityEditor;

namespace NullFramework.Editor
{
    //TODO:把框架和游戏主体生成节点放置在不同的位置
    public class NodeBuilder 
    {
        
        //private static string m_saveFloder = $"{Application.dataPath}/Code/NullFramework/Game/BehaviorVM/Editor/Auto";
        private static string[] m_assemblyNames = new string[]
        {
            "MainPart",
            "NullFramework",
        };
        public static string DefalutContent()
        {
            string content = @"
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using NullFramework.Editor;

namespace {assembly_name}.Editor
{
    public class {node_name} : ByteNode
    {
{var_fileds}
        protected override void Init()
        {
            base.Init();
            //Desc = $""{desc}"";
        }
        public override List<int> GetCode(bool isPositve)
        {
            List<int> list = new List<int>();

{var_codes}
            return list;
        }
    }
    
}";
            return content;
        }

        private static string GetFloder(string assemblyName)
        {
            return $"{Application.dataPath}/Code/{assemblyName}/Game/BehaviorVM/Editor/Auto";
        }
        [MenuItem("Tool/生成技能节点")]
        public static void BulidNode()
        {
            for (int i = 0; i < m_assemblyNames.Length; i++)
            {
                var name = m_assemblyNames[i];
                var floder = GetFloder(name);
                ClearFloder(floder);
                var assembly = Assembly.Load(name);
                var types = assembly.GetTypes();
                var baseType = typeof(NullFramework.Runtime.ByteCodeBehaviorVM);
                foreach (var type in types)
                {
                    //type是baseType的父类或 type == baseType 返回true
                    if(!baseType.IsAssignableFrom(type))
                    {
                        continue;
                    }
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < methods.Length; j++)
                    {
                        BuilderNode(methods[j], floder, name);
                    }
                }
            }
            
            AssetDatabase.Refresh();
        }

        

        [MenuItem("Tool/清除技能节点")]
        public static void ClearNode()
        {
            
            for (int i = 0; i < m_assemblyNames.Length; i++)
            {
                var name = m_assemblyNames[i];
                var floder = GetFloder(name);
                ClearFloder(floder);
            }
            AssetDatabase.Refresh();
        }

        private static void ClearFloder(string floder)
        {
            if(!Directory.Exists(floder))
            {
                Directory.CreateDirectory(floder);
            }
            else
            {
                var fs = Directory.GetFiles(floder);
                for (int i = 0; i < fs.Length; i++)
                {
                    File.Delete(fs[i]);
                }
            }
            
        }

        public static void BuilderNode(MethodInfo method, string floder, string assemblyName)
        {   
            var skillInfo = method.GetCustomAttribute<NullFramework.Runtime.ByteCodeAttribute>();
            bool isCreate = skillInfo?.IsAuto ?? false;
            if(!isCreate)
            {
                return;
            }
            string var_fileds = "";
            string var_codes = "";
            string method_name = method.Name;
            string desc = skillInfo.Desc;
            string node_name = $"{method.Name}Node";
            string content = DefalutContent().Replace("{assembly_name}",assemblyName);
            if(skillInfo.IsHasNext)
            {
                var_fileds += "\t\t[Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]\n" +
                              "\t\tpublic byte Next = 0;\n";
            }

            if(skillInfo.Ports.Length > 0)
            {
                int cnt = skillInfo.Ports.Length;
                for (int i = 0; i < cnt; i++)
                {
                    var port = skillInfo.Ports[i];
                    var_fileds += "\t\t[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]\n" +
                                   $"\t\tpublic byte {port} = 0;\n";
                    var_codes = $"\t\t\tvar c{i} = GetInputCode(\"{port}\");\n" + $"\t\t\tlist.AddRange(c{i});\n{var_codes}"; 
                }
            }

            if(skillInfo.IsHasEnter)
            {
                var_fileds += "\t\t[Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]\n" +
                              "\t\tpublic byte Enter = 0;\n";
            }   
            var_codes = $"{var_codes}\t\t\tlist.Add({skillInfo.Id});\n"; 
            if(skillInfo.IsHasEnter)
            {
                string str = $"\t\t\t\tvar c_enter = GetInputCode(\"Enter\");\n" + $"\t\t\t\tlist.AddRange(c_enter);\n"; 
               str = $"\t\t\tif(!isPositve)\n\t\t\t{{\n{str}\t\t\t}}\n";
                var_codes = $"{str}{var_codes}";
            }
            if(skillInfo.IsHasNext)
            {
                string str = $"\t\t\t\tvar c_next = GetOutputCode(\"Next\");\n" + $"\t\t\t\tlist.AddRange(c_next);\n"; 
                str = $"\t\t\tif(isPositve)\n\t\t\t{{\n{str}\t\t\t}}\n";
                var_codes = $"{var_codes}{str}";
            }
                                
            content = content.Replace("{var_fileds}", var_fileds).Replace("{method_name}", method_name)
                             .Replace("{desc}", desc).Replace("{node_name}", node_name)
                             .Replace("{var_codes}", var_codes);
            string path = $"{floder}/{node_name}.cs";
            File.WriteAllText(path, content);    
        }
    }
}
