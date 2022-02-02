using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public class FileUtility 
    {
        public static string LocalPathToAbsPath(string path)
        {
            if(path == null || !path.StartsWith("Assets/"))
            {
                return null;
            }
            return Application.dataPath + path.Substring(6);
        }
    }
}

