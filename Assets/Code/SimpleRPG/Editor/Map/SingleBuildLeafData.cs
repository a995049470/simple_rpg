using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    [CreateAssetMenu(fileName = "SingleBuildLeafData", menuName = "MapEditor/SingleBuildLeafData")]
    public class SingleBuildLeafData : LeafData<SingleBuildLeaf>, IFSMLeafData
    {
        [SerializeField]
        private GameObject defalutPrefab;
        public GameObject DefalutPrefab { get=> defalutPrefab; }
        [SerializeField]
        private Vector3Int spacing;
        public Vector3Int Spacing { get => spacing; }

        public int GetLeafKind()
        {
            return ((int)OperateType.SingleBuild);
        }
    }

}

