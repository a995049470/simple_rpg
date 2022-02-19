using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "PlayerTressData", menuName = "SimpleRPG/PlayerTressData")]
    public class PlayerTressData : LeafData<PlayerTress>
    {
        [SerializeField]
        private GameObject playerPrefab;

        public GameObject InstantiatePlayer()
        {
            var go = UObjectUtility.InstantiateGameObject(playerPrefab);
            return go;
        }

    }
}


