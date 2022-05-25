using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    [CreateAssetMenu(fileName = "UnitTressData", menuName = "SimpleRPG/UnitTressData", order = 0)]
    public class UnitTressData : LeafData<UnitTress>, ITRSSetter{
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        public UnitKind unitKind;
        [SerializeField]
        public int hp;
        [SerializeField]
        public int atk;
        
        public GameObject InstantiateUnit()
        {
            var go = UObjectUtility.InstantiateGameObject(playerPrefab);
            go.transform.position = position;
            return go;
        }

        public void SetTRS(Vector3 _position, Quaternion _rotation, Vector3 _scale)
        {
            position = _position;
        }

        public AbilityData CreateAblityData()
        {
            var data = new AbilityData();
            data.hp = hp;
            data.currentHp = hp;
            data.unitKind = ((int)unitKind);
            return data;
        }
    }
}


