using UnityEngine;

namespace NullFramework.Runtime
{
    [CreateAssetMenu(fileName = "RigidLeafData", menuName = "NullFramework/RigidLeafData")]
    public class RigidLeafData : LeafData<RigidLeaf>
    {
        [SerializeField]
        private float mass;
        public float Mass { get => Mathf.Max(mass, 0.01f); }
        [Range(0, 1)]
        public float elasticity;
        [Range(0, 1)]
        public float u_t;
    }
}

