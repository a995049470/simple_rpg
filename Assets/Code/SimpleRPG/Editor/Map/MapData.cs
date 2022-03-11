using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NullFramework.Editor
{
    public class MapData
    {
        private Dictionary<Vector3Int, GameObject> buildings = new Dictionary<Vector3Int, GameObject>();
        private GameObject currentPrefab;
        private GameObject mapRoot;
        
        public MapData()
        {
            mapRoot = new GameObject("MapRoot");
        }
        
        public void SetCurrentPrefab(GameObject prefab)
        {
            this.currentPrefab = prefab;
        }

        //尝试增加建筑
        public bool TrayAddBuild(Vector3Int pos)
        {
            if(buildings.ContainsKey(pos)) return false;
            if(currentPrefab == null) return false;
            var building = GameObject.Instantiate(currentPrefab, pos, Quaternion.identity);
            building.transform.parent = mapRoot.transform;
            buildings[pos] = building;
            return true;
        }

        public void OnEditorFinish()
        {
            GameObject.DestroyImmediate(mapRoot);
        }
    }

}

