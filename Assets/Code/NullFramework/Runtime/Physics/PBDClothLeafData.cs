using UnityEngine;

namespace NullFramework.Runtime
{
    [CreateAssetMenu(fileName = "PBDClothLeafData", menuName = "NullFramework/PBDClothLeafData")]
    public class PBDClothLeafData : LeafData<PBDClothLeaf>, ITRSSetter
    {
        public Material material;
        public SphereModel sphere { get => GameObject.Find(sphereName).GetComponent<SphereModel>(); }
        [SerializeField]
        private string sphereName;
        [Range(0, 64)]
        public int iterate = 16;
        [Range(1, 32)]
        public int numX = 21;
        [Range(1, 32)]
        public int numY = 21;
        public float weight = 0.2f;
        public Vector3 originPosition;

        public void SetTRS(Vector3 _position, Quaternion _rotation, Vector3 _scale)
        {
            originPosition = _position;
        }
    }
}

