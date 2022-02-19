using UnityEngine;
namespace NullFramework.Runtime
{
    public static class UObjectUtility
    {
        public static GameObject InstantiateGameObject(GameObject prefab)
        {
        #if UNITY_EDITOR
            return UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        #else
            return GameObject.Instantiate(prefab);
        #endif
        }
    }
}