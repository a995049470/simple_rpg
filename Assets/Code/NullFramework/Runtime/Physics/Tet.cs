using UnityEngine;

namespace NullFramework.Runtime
{
    public class Tet
    {
        private const int count = 4;
        public int[] id = new int[count];
        public Matrix4x4 dm;
        public Matrix4x4 dm_i;
        public Matrix4x4 dm_ti;
        public float det_dm_i;

        public void Init(Vector3[] vertices)
        {
            var e10 = -vertices[id[1]] + vertices[id[0]]; 
            var e20 = -vertices[id[2]] + vertices[id[0]]; 
            var e30 = -vertices[id[3]] + vertices[id[0]];
            dm = new Matrix4x4(e10, e20, e30, Vector3.zero);
            dm[3, 3] = 1;
            dm_i = dm.inverse;
            dm_ti = dm.inverse.transpose;
            det_dm_i = Matrix4x4.Determinant(dm_i);
        }


        public Matrix4x4 GetDeformation(Vector3[] vertices)
        {
            var e10 = -vertices[id[1]] + vertices[id[0]];
            var e20 = -vertices[id[2]] + vertices[id[0]];
            var e30 = -vertices[id[3]] + vertices[id[0]];
            var m = new Matrix4x4(e10, e20, e30, Vector4.zero);
            m[3, 3] = 1;
            var f = m * dm_i;
            return f;
        }
    }
}

