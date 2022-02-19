using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "MoveLeaf", menuName = "SimpleRPG/MoveLeaf")]
    public class MoveLeafData : LeafData<MoveLeaf>
    {
        public float moveSpeed = 4;
    }
}

