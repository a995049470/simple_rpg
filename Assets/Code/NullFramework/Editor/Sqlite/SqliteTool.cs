using System.Collections;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System;
using System.Reflection;
using LitJson;
namespace NullFramework.Editor
{
    public class SqliteTool
    {
        private static string c_excelPath = $"{Application.dataPath}/~Doc/Excel";
        private static string c_scriptPath = $"{Application.dataPath}/Code/MainPart/Game/Data/Runtime/Auto";
        private static string c_tempPath = $"{Application.dataPath}/Code/NullFramework/Game/Sqlite/Editor/temp.txt";
        private static string c_dbRoot = $"{Application.streamingAssetsPath}/Data";
        private static string c_dbPath = $"{Application.streamingAssetsPath}/Data/local.db";
        private static string c_currentAssemblyName = "MainPart";
        private static SQLiteConnection connection;
        [MenuItem("Tool/数据类生成")]
        private static void AutoGenerateScript()
        {
            AutoExcelsToScript();
        }

         [MenuItem("Tool/数据入库")]
        private static void AutoGenerateData()
        {
            CreateDatabase();
        }
        
        private static void AutoExcelsToScript()
        {
            if (Directory.Exists(c_scriptPath))
            {
                Directory.Delete(c_scriptPath, true);
            }
            Directory.CreateDirectory(c_scriptPath);
            DirectoryInfo dir = new DirectoryInfo(c_excelPath);
            var excels = dir.GetFiles("*.xlsx");
            for (int i = 0; i < excels.Length; i++)
            {
                ExcelToScript(excels[i].FullName);
            }
            AssetDatabase.Refresh();
            Debug.Log("生成脚本完毕");
        }

        
        private static void CreateDatabase()
        {
            if(!Directory.Exists(c_dbRoot))
            {
                Directory.CreateDirectory(c_dbRoot);
            }
            if(File.Exists(c_dbPath))
            {
                File.Delete(c_dbPath);
            }
            NullFramework.Runtime.SqliteManager.Clear();
            connection = new SQLiteConnection(c_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            DirectoryInfo dir = new DirectoryInfo(c_excelPath);
            var excels = dir.GetFiles("*.xlsx");

            for (int i = 0; i < excels.Length; i++)
            {
                AddTableToDatabase(excels[i].FullName);

            }
            connection.Close();
            AssetDatabase.Refresh();
            Debug.Log("导入数据库完毕");
        }

        private static void AddTableToDatabase(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            ExcelPackage package = new ExcelPackage(fs);
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            var row_name = sheet.Dimension.Start.Row + 1;
            string type_data = new DirectoryInfo(path).Name.Replace(".xlsx", "");
            Assembly ass = Assembly.Load(c_currentAssemblyName);
            Type type = ass.GetType(type_data);
            if(type == null)
            {
                return;
            }
            List<string> nameList = new List<string>();
            for (int i = sheet.Dimension.Start.Column; i <= sheet.Dimension.End.Column; i++)
            {
                string name = sheet.GetValue(row_name, i)?.ToString();
                if (string.IsNullOrEmpty(name) || name.StartsWith("//"))
                {
                    continue;
                }
                nameList.Add(name);
            }
            List<object> objList = new List<object>();
            int row_start = sheet.Dimension.Start.Row + 2;
            int row_end = sheet.Dimension.End.Row + 1;
            int col_start = sheet.Dimension.Start.Column;
            int col_end = sheet.Dimension.End.Row + 1;
            var flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.SetProperty;
            for (int i = row_start; i < row_end; i++)
            {
                object obj = type.Assembly.CreateInstance(type.Name);
                string firstvalue = sheet.GetValue(i, col_start)?.ToString();
                if (string.IsNullOrEmpty(firstvalue))
                {
                    continue;
                }
                for (int j = col_start; j < col_end; j++)
                {
                    int index = j - col_start;
                    if (index >= nameList.Count)
                    {
                        break;
                    }
                    string value = sheet.GetValue(i, j)?.ToString();
                    var name = nameList[index];
                    var property = type.GetProperty(name, flag);
                    SetPropertyValue(obj, property, value);
                }
                objList.Add(obj);
            }
            connection.DropTable(type);
            connection.CreateTable(type);
            connection.InsertAll(objList.ToArray());
            sheet.Dispose();
            package.Dispose();
            fs.Close();
        }

        private static void SetPropertyValue(object obj, PropertyInfo info, string str)
        {

            var type_filed = info.PropertyType;
            object value = null;
            bool isNullOrEmpty = string.IsNullOrEmpty(str);
            if (type_filed == typeof(int))
            {
                if (isNullOrEmpty) value = 0;
                else value = int.Parse(str);
            }
            else if (type_filed == typeof(float))
            {
                if (isNullOrEmpty) value = 0.0f;
                else value = float.Parse(str);
            }
            else if (type_filed == typeof(string))
            {
                if (isNullOrEmpty) value = "";
                else value = str;
            }
            else if (type_filed == typeof(int[]))
            {
                if (isNullOrEmpty) value = new int[0];
                else value = JsonMapper.ToObject<int[]>(str);
            }
            else if (type_filed == typeof(string[]))
            {
                if (isNullOrEmpty) value = new string[0];
                else value = JsonMapper.ToObject<string[]>(str);
            }
            else if (type_filed == typeof(float[]))
            {
                if (isNullOrEmpty) value = new float[0];
                else value = JsonMapper.ToObject<float[]>(str);
            }
            info.SetValue(obj, value);
        }

        private static void ExcelToScript(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            ExcelPackage package = new ExcelPackage(fs);
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            var rowStart = sheet.Dimension.Start.Row;
            bool isFirst = true;
            string var_data = "";
            string type_data = new DirectoryInfo(path).Name.Replace(".xlsx", "");
            string content = File.ReadAllText(c_tempPath);

            for (int i = sheet.Dimension.Start.Column; i <= sheet.Dimension.End.Column; i++)
            {
                string type = sheet.GetValue(rowStart, i)?.ToString();
                string name = sheet.GetValue(rowStart + 1, i)?.ToString();
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name) || type.StartsWith("//") || name.StartsWith("//"))
                {
                    continue;
                }
                string filed = $"\tpublic {type} {name}{{ get; set; }}\n";
                if (isFirst)
                {
                    isFirst = false;
                    filed = $"\t[PrimaryKey]\n{filed}";
                }

                var_data += filed;
            }
            content = content.Replace("{var_data}", var_data).Replace("{type_data}", type_data);
            string outpath = $"{c_scriptPath}/{type_data}.cs";
            File.WriteAllText(outpath, content);
            fs.Close();
            package.Dispose();
            sheet.Dispose();
        }

    }

}
