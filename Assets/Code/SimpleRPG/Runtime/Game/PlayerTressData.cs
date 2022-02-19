using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "PlayerTressData", menuName = "SimpleRPG/PlayerTressData")]
    public class PlayerTressData : LeafData<PlayerTress>
    {
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private Vector3 bornPos;

        public GameObject InstantiatePlayer()
        {
            var go = Instantiate(playerPrefab);
            go.transform.SetPositionAndRotation(bornPos, Quaternion.identity);
            return go;
        }

    }
}


