using UnityEngine;
namespace NullFramework.Runtime
{
    public static class LRes
    {
        private const string luaFloder = "Assets/GameResource/Lua";
        public static byte[] LoadLuaBytes(string fileName)
        {
            var path = $"{luaFloder}/{fileName}.lua";
            byte[] bytes = null;
        #if UNITY_EDITOR
            var script = UnityEditor.AssetDatabase.LoadAssetAtPath<LuaAsset>(path).text;
            bytes = System.Text.Encoding.UTF8.GetBytes(script);
        #endif
            return bytes;
        } 
    }
}