using UnityEngine;

namespace NullFramework.Runtime
{

    [CreateAssetMenu(fileName = "ShapeMatchingRigidLeafData", menuName = "NullFramework/ShapeMatchingRigidLeafData")]
    public class ShapeMatchingRigidLeafData : LeafData<ShapeMatchingRigidLeaf>
    {
        [Range(0, 1)]
        public float elasticity;
        [Range(0, 1)]
        public float u_t;
    }
}

