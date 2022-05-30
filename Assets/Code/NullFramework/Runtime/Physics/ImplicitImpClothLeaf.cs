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
        private float edge { get => 4.0f / numX; }
        private float diagEdge { get => 1.424f * edge; }
        private float bigMass = 1E+20f;
        private Mesh mesh;
        private float[] mass;
        private Spring[] springs;
        private Material clothMaterial;
        private float damping = 0.99f;
        private float k = 2000;
        private int iterate = 16;
        private Vector3 g = 9.8f * Vector3.down;
        private Vector3[] vertices;
        private Vector3[] originVertices;
        private Vector3[] gradient;
        private Vector3[] velocity;
        private SphereModel sphere;
        
        
        

        public override void OnReciveDataFinish()
        {
            base.OnReciveDataFinish();
            clothMaterial = leafData.material;
            sphere = leafData.sphere;
            iterate = leafData.iterate;
            k = leafData.k;
            numX = leafData.numX;
            numY = leafData.numY;
        }

        public override void OnEnable(Leaf lastLeaf = null)
        {
            base.OnEnable(lastLeaf);
            Init();
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(BaseMsgKind.RigidUpdate, RigidUpdate);
        }

        //衣服的系统更新
        private System.Action RigidUpdate(Msg msg)
        {
            if(mesh == null) return emptyAction;
            int vertCount = numX * numY;
            Vector3 a = Vector3.zero;
            float dt = root.FrameDeltaTime;
            //先备份一下顶点坐标
            g = 9.8f * Vector3.down;
            //1.更新v和x
            for (int i = 0; i < vertCount; i++)
            {
                if(!IsFixedPoint(i))
                {
                    velocity[i] = (velocity[i]) * damping;
                    vertices[i] = vertices[i] + velocity[i] * dt;
                    originVertices[i] = vertices[i];
                }
            }
            //2.牛顿法迭代求得弹性势能最小的位置 更新v和x
            var springCount = springs.Length;
            for (int n = 0; n < iterate; n++)
            {
                for (int i = 0; i < vertCount; i++)
                {
                    var dx = vertices[i] - originVertices[i];
                    gradient[i] = mass[i] * (dx) / dt / dt - mass[i] * g; 
                }
                for (int i = 0; i < springCount; i++)
                {
                    var spring = springs[i];
                    var pi = vertices[spring.i];
                    var pj = vertices[spring.j];
                    var currentLength = Mathf.Max(Vector3.Distance(pi, pj), 0.01f);
                    var gi = k * ( 1 - spring.length / currentLength) * (pi - pj);
                    var gj = k * ( 1 - spring.length / currentLength) * (pj - pi);
                    gradient[spring.i] += gi;
                    gradient[spring.j] += gj;
                }
                
                for (int i = 0; i < vertCount; i++)
                {
                    if(!IsFixedPoint(i))
                    {
                        vertices[i] -= 1 / (1 / dt / dt * mass[i] + 4 * k) * gradient[i];
                    }
                }
            }

            //更新速度
            for (int i = 0; i < vertCount; i++)
            {
                velocity[i] += (vertices[i] - originVertices[i]) / dt;
            }

            //3.处理碰撞 更新 v和x
            var center = sphere?.center ?? Vector3.zero;
            var radius = sphere?.radius ?? 0;

            for (int i = 0; i < vertCount; i++)
            {
                var vert = vertices[i];
                vert = center + (vert - center).normalized * Mathf.Max((vert - center).magnitude, radius);
                velocity[i] += (vert - vertices[i]) / dt;
                vertices[i] = vert;
            }

            //更新mesh
            mesh.vertices = vertices;
            return emptyAction;
        }

        private bool IsFixedPoint(int i){
            return i == 0 || i == numX - 1;
        }

        private void Init()
        {
            var vertexCount = numX * numY;
            var indiceCount = 3 * 2 * (numX - 1) * (numY - 1);
            var springCount = ((numX - 2) * (numY - 2) * 8 + 4 * 3 + (2 * numX - 4 + 2 * numY - 4) * 5) / 2;
            vertices = new Vector3[vertexCount];
            originVertices = new Vector3[vertexCount];
            gradient = new Vector3[vertexCount];
            velocity = new Vector3[vertexCount];
            var uv = new Vector2[vertexCount];
            var indices = new int[indiceCount];
            springs = new Spring[springCount];
            mass = new float[vertexCount];
            var vz = 0;
            int start_indices = 0;
            int start_spring = 0;
            mesh = new Mesh();
            float xMin = -0.5f * numX * edge;
            float xMax = 0.5f * numX * edge;
            float yMin = -0.5f * numY * edge;
            float yMax = 0.5f * numY * edge;
            for (int i = 0; i < vertexCount; i++)
            {
                mass[i] = 1;
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

