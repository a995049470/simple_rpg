using UnityEngine;

namespace NullFramework.Runtime
{
    public struct Spring
    {
        //两端
        public int i;
        public int j;
        //原长
        public float length;

        public Spring(int _i, int _j, float _len)
        {
            i = _i;
            j = _j;
            length = _len;
        }
    }

    public class ImplicitImpClothLeaf : Leaf<ImplicitImpClothLeafData>
    {
        private int numX = 21;
        private int numY = 21;
        private const float halfEdge = 0.1f;
        private const float edge = halfEdge * 2;
        private const float diagEdge = halfEdge * 2.8284f;
        private float xMin = -halfEdge;
        private float xMax = halfEdge;
        private float yMin = -halfEdge;
        private float yMax = halfEdge;
        private Mesh mesh;
        private float[] mass;
        private Spring[] springs;
        private Material clothMaterial;
        private float damping = 0.99f;
        private float k = 2000;
        

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            clothMaterial = leafData.material;
        }

        public override void OnEnable(Leaf lastLeaf = null)
        {
            base.OnEnable(lastLeaf);
            Init();
        }

        //衣服的系统更新
        private System.Action RigidUpdate(Msg msg)
        {
            if(mesh == null) return emptyAction;
            //1.初步跟新v和x
            //2.牛顿法迭代求得弹性势能最小的位置 更新v和x
            //3.处理碰撞 更新 v和x
            
            return emptyAction;
        }

        private void Init()
        {
            var vertexCount = numX * numY;
            var indiceCount = 3 * 2 * (numX - 1) * (numY - 1);
            var springCount = ((numX - 2) * (numY - 2) * 8 + 4 * 3 + (2 * numX - 4 + 2 * numY - 4) * 5) / 2;
            var vertices = new Vector3[vertexCount];
            var uv = new Vector2[vertexCount];
            var indices = new int[indiceCount];
            springs = new Spring[springCount];
            var vz = 0;
            int start_indices = 0;
            int start_spring = 0;
            mesh = new Mesh();

            for (int i = 0; i < vertexCount; i++)
            {
                //填充顶点和uv
                int x = i % numX;
                int y = i / numX;
                var tx = (float)x / (numX - 1);
                var ty = (float)y / (numY - 1);
                var vx = Mathf.Lerp(xMin, xMax, tx);
                var vy = Mathf.Lerp(yMax, yMin, ty);
                var vert = new Vector3(vx, vy, vz);
                vertices[i] = vert;
                uv[i] = new Vector2(tx, ty);
                //填充序列
                if(x != numX - 1 && y != numY - 1)
                {
                    indices[start_indices ++] = i;
                    indices[start_indices ++] = i + numX + 1;
                    indices[start_indices ++] = i + numX;
                    indices[start_indices ++] = i;
                    indices[start_indices ++] = i + 1;
                    indices[start_indices ++] = i + numX + 1;
                }
                
                //填充弹簧
                bool hasR = x < numX - 1;
                bool hasL = x > 0;
                bool hasB = y < numY - 1;
                bool hasBR = hasB & hasR;
                bool hasBL = hasB & hasL;

                if(hasR) 
                {
                    springs[start_spring ++ ] = new Spring(i, i + 1, edge);
                }
                if(hasB)
                {
                    springs[start_spring ++] = new Spring(i, i + numX, edge);
                }
                if(hasBR)
                {
                    springs[start_spring ++] = new Spring(i, i + numX + 1, diagEdge);
                }
                if(hasBL)
                {
                    springs[start_spring ++] = new Spring(i, i + numX - 1, diagEdge);
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            
            var go = new GameObject("cloth");
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = clothMaterial;        
            
        }
        
    }
}

