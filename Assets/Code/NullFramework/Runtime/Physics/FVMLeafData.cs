using UnityEngine;

namespace NullFramework.Runtime
{
    [CreateAssetMenu(fileName = "FVMLeafData", menuName = "NullFramework/FVMLeafData")]
    public class FVMLeafData : LeafData<FVMLeaf>, ITRSSetter
    {
        public Material material;
        public string elePath;
        public string nodePath;
        public Vector3 position;

        public void SetTRS(Vector3 _position, Quaternion _rotation, Vector3 _scale)
        {
            position = _position;
        }
    }
}

