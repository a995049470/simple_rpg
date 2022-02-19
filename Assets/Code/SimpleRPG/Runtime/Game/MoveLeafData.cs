using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "MoveLeafData", menuName = "SimpleRPG/MoveLeafData")]
    public class MoveLeafData : LeafData<MoveLeaf>
    {
        [SerializeField]
        private float moveSpeed = 4;
        public float MoveSpeed { get => moveSpeed; }

    }
}

