using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "CameraFollowLeafData", menuName = "SimpleRPG/CameraFollowLeafData")]
    public class CameraFollowLeafData : LeafData<CameraFollowLeaf>
    {
        [SerializeField]
        private GameObject cameraPrefab;
        public float zdistance = 8;
        [Range(0, 1)]
        public float smoothTime = 0.5f;
        public float maxSpeed = 10;
        public GameObject InstantiateCamera()
        {
            return UObjectUtility.InstantiateGameObject(cameraPrefab);
        }
    }
}

