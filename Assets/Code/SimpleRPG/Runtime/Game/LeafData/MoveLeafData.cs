using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{

    [CreateAssetMenu(fileName = "MoveLeafData", menuName = "SimpleRPG/MoveLeafData")]
    public class MoveLeafData : LeafData<MoveLeaf>
    {
        public float moveSpeed;
    }
}

