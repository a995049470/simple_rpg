using UnityEngine;

namespace NullFramework.Runtime
{
    public class Tet
    {
        private const int count = 4;
        public int[] id = new int[count];
        public Matrix4x4 dm_i = default;
        public Matrix4x4 dm_ti = default;
        public float det_dm_i = 0;
        public float Vref_Dm;

        private Matrix4x4 CalEdgeMatrix(Vector3[] vertices)
        {
            Matrix4x4 ret = Matrix4x4.zero;
            Vector3[] edge = new Vector3[3];
            edge[0] = vertices[id[0]] - vertices[id[1]];
            edge[1] = vertices[id[0]] - vertices[id[2]];
            edge[2] = vertices[id[0]] - vertices[id[3]];
            ret[0, 0] = edge[0].x;
            ret[0, 1] = edge[1].x;
            ret[0, 2] = edge[2].x;
            ret[1, 0] = edge[0].y;
            ret[1, 1] = edge[1].y;
            ret[1, 2] = edge[2].y;
            ret[2, 0] = edge[0].z;
            ret[2, 1] = edge[1].z;
            ret[2, 2] = edge[2].z;
            ret[3, 3] = 1.0f;
            return ret;
        }

        public void Init(Vector3[] vertices)
        {
            var dm = CalEdgeMatrix(vertices);
            dm_i = dm.inverse;
            dm_ti = dm.inverse.transpose;
            det_dm_i = dm.inverse.determinant;
            Vref_Dm = -1.0f / (6.0f * dm_i.determinant);
        }


        public Matrix4x4 GetDeformation(Vector3[] vertices)
        {
            var edge = CalEdgeMatrix(vertices);
            var f = edge * dm_i;
            return f;
        }
    }
}

