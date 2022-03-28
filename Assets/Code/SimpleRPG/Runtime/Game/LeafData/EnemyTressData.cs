using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "EnemyTressData", menuName = "SimpleRPG/EnemyTressData")]
    public class EnemyTressData : LeafData<EnemyTress>, ITRSSetter
    {
        [SerializeField]
        private GameObject enemyPrefab;
        [SerializeField]
        private Vector3 position;

        public GameObject InstantiateEnemy()
        {
            var go = UObjectUtility.InstantiateGameObject(enemyPrefab);
            go.transform.position = position;
            return go;
        }

        public void SetTRS(Vector3 _position, Quaternion _rotation, Vector3 _scale)
        {
            this.position = _position;
        }
    }
}


