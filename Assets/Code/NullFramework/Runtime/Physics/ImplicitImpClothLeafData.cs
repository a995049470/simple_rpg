using UnityEngine;

namespace NullFramework.Runtime
{
    [CreateAssetMenu(fileName = "ImplicitImpClothLeafData", menuName = "NullFramework/ImplicitImpClothLeafData")]
    public class ImplicitImpClothLeafData : LeafData<ImplicitImpClothLeaf>
    {
        public Material material;
    }
}

