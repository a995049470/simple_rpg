using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    [CreateAssetMenu(fileName = "PlayerTressData", menuName = "SimpleRPG/PlayerTressData")]
    public class PlayerTressData : LeafData<PlayerTress>, ITRSSetter
    {
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private Vector3 position;
        public GameObject InstantiatePlayer()
        {
            var go = UObjectUtility.InstantiateGameObject(playerPrefab);
            go.transform.position = position;
            return go;
        }

        public void SetTRS(Vector3 _position, Quaternion _rotation, Vector3 _scale)
        {
            position = _position;
        }
    }
}


