using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    [CreateAssetMenu(fileName = "OperateManagerTressData", menuName = "MapEditor/OperateManagerTressData")]
    public class OperateManagerTressData : LeafData<OperateManagerTress> 
    {
        [SerializeField]
        private GameObject defaultPrefab;
        public GameObject DefaultPrefab { get => defaultPrefab; }
    }

}

