using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    [CreateAssetMenu(fileName = "BuildLeafData", menuName = "MapEditor/BuildLeafData")]
    public class BuildLeafData : LeafData<BuildLeaf>
    {
        [SerializeField]
        private GameObject defalutPrefab;
        public GameObject DefalutPrefab { get=> defalutPrefab; }
        [SerializeField]
        private Vector3Int spacing;
        public Vector3Int Spacing { get => spacing; }
        
    }

}

