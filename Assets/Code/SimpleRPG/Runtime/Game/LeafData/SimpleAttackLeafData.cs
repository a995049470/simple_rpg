using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    [CreateAssetMenu(fileName = "SimpleAttackLeafData", menuName = "SimpleRPG/SimpleAttackLeafData")]
    public class SimpleAttackLeafData : LeafData<SimpleAttackLeaf>
    {
        public string luaFile;
        public float attackRadius;
        public int attackNum;
    
    }
}

