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
        private GameObject preview;
        private bool isDityPreviw;
        
        public MapData()
        {
            mapRoot = new GameObject("MapRoot");
        }
        
        public void SetCurrentPrefab(GameObject prefab)
        {
            if(prefab != currentPrefab) isDityPreviw = true;
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
        //尝试删除建筑
        public bool TrayRmoveBuild(Vector3Int pos)
        {
            if(!buildings.ContainsKey(pos)) return false;
            var building = buildings[pos];
            GameObject.DestroyImmediate(building);
            buildings.Remove(pos);
            return true;
        }

        public void ShowPreview(Vector3 pos)
        {
            if(isDityPreviw || preview == null) 
            {
                isDityPreviw = false;
                if(preview != null)
                {
                    GameObject.DestroyImmediate(preview);
                }
                preview = GameObject.Instantiate(currentPrefab, pos, Quaternion.identity);
                preview.transform.parent = mapRoot.transform;
            }
            preview.transform.position = pos;
        }

        public void OnEditorFinish()
        {
            GameObject.DestroyImmediate(mapRoot);
        }
    }

}

