using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    public struct TriangleBarrier
    {
        public uint indexId;
        public uint matrixId;

        public const int Size = 2 * sizeof(uint);
    }

    [CreateAssetMenu(fileName = "MetaCellGIPass", menuName = "LPipeline/Passes/MetaCellGIPass")]
    public class MetaCellGIPass : RenderPass
    {
        private bool isDity = true;
        //范围
        [SerializeField]
        private Vector3 origin = new Vector3(-32, -32, -32);
        [SerializeField]
        private Vector3Int blockNum = new Vector3Int(128, 128, 128);
        [SerializeField]
        private Vector3 blockSize = new Vector3(0.5f, 0.5f, 0.5f);

        //障碍相关
        private ComputeBuffer barrierBuffer;
        private Vector3Int barrierBufferSize { get => blockNum; }
        private int barrierBufferCount { get => blockNum.x * blockNum.y * blockNum.z; }
        private const int barrierStride = 3 * sizeof(float);

        //把所有障碍物分解成三角形
        private Dictionary<Mesh, Vector2Int> meshStartIdDic;
        private ComputeBuffer verticesBuffer;
        private ComputeBuffer indicesBuffer;
        private const int vertStride = 3 * sizeof(float);
        private const int matrixStride = 16 * sizeof(float);
        //灯光亮度相关
        private RenderTexture lightColorBuffer;
        private RenderTexture currentGlobalLightColorBuffer;
        private RenderTexture[] globalLightColorBuffer;
        private int currentGlobalLightColorBufferIndex;

        //计算着色器相关
        [SerializeField]
        private ComputeShader cs;
        private int kernel_computeBarrier;
        private const string name_computeBarrier = "ComputeBarrier";
        private int kernel_clearBarrier;
        private const string name_clearBarrier = "ClearBarrier";
        private const int NumThreadX = 8;
        private const int NumThreadY = 8;
        private const int NumThreadZ = 1;
        private static int nameId_BarrierBuffer = Shader.PropertyToID("_BarrierBuffer");
        private static int nameId_BlockNum = Shader.PropertyToID("_BlockNum");
        private static int nameId_BlockSize = Shader.PropertyToID("_BlockSize");
        private static int nameId_Origin = Shader.PropertyToID("_Origin");
        private static int nameId_VerticesBuffer = Shader.PropertyToID("_VerticesBuffer");
        private static int nameId_IndicesBuffer = Shader.PropertyToID("_IndicesBuffer");
        private static int nameId_MatrixBuffer = Shader.PropertyToID("_MatrixBuffer");
        private static int nameId_TriangleBarrierBuffer = Shader.PropertyToID("_TriangleBarrierBuffer");
        private static int nameId_TriangleCount = Shader.PropertyToID("_TriangleCount");

        private void Init()
        {
            if(!isDity) return;
            isDity = false;
            meshStartIdDic = new Dictionary<Mesh, Vector2Int>();
            barrierBuffer = new ComputeBuffer(barrierBufferCount, barrierStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            verticesBuffer = new ComputeBuffer(1, vertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            indicesBuffer = new ComputeBuffer(1, vertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            kernel_computeBarrier = cs?.FindKernel(name_computeBarrier) ?? 0;
            kernel_clearBarrier = cs?.FindKernel(name_clearBarrier) ?? 0;
        }

        private void OnEnable()
        {
           
        }



        private void OnValidate() {
           isDity = true;
        }

        private void OnDisable()
        {
            barrierBuffer?.Release();
            indicesBuffer?.Release();
            verticesBuffer?.Release();
            
        }

        private Vector3Int GetGroup(int count)
        {
            float g = (float)count / NumThreadX
             / NumThreadY;
            int gx = Mathf.CeilToInt(Mathf.Sqrt(g));
            int gy = gx;
            int gz = 1;
            return new Vector3Int(gx, gy, gz);
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            Init();
            //准备computeBuffer使用的数据
            var barrierDic = MetaCellGIBarrierManager.Instance.GetBarrierDic();
            UpdateVerticesBuffer(barrierDic.Keys);
            int triangleTotalCount = 0;
            foreach (var kvp in barrierDic)
            {
                triangleTotalCount += (int)kvp.Key.GetIndexCount(0) / 3 * kvp.Value.Count;
            }

            //清理屏障值
            {
                cs.SetBuffer(kernel_clearBarrier, nameId_BarrierBuffer, barrierBuffer);
                var group1 = GetGroup(barrierBufferCount);
                cs.Dispatch(kernel_clearBarrier, group1.x, group1.y, group1.z);
            }

            //设置屏障值
            if(triangleTotalCount > 0)
            {
                var triangles = new TriangleBarrier[triangleTotalCount];
                int sum_matrixCount = 0;
                int sum_triangleCount = 0;
                var matrixList = new List<Matrix4x4>();
                foreach (var kvp in barrierDic)
                {
                    var mesh = kvp.Key;
                    var indexStartId = meshStartIdDic[mesh].y;
                    var triangleCount = (int)mesh.GetIndexCount(0) / 3;
                    var matrixCount = kvp.Value.Count;
                    matrixList.AddRange(kvp.Value);
                    for (int i = 0; i < matrixCount; i++)
                    {
                        for (int j = 0; j < triangleCount; j++)
                        {
                            var triangleId = sum_triangleCount + j;
                            triangles[triangleId].indexId = (uint)(indexStartId + j * 3);
                            triangles[triangleId].matrixId = ((uint)sum_matrixCount);
                        }      
                        sum_triangleCount += triangleCount;
                        sum_matrixCount += 1;              
                    }
                }
                
                var triangleBarriarBuffer = new ComputeBuffer((int)triangleTotalCount, TriangleBarrier.Size, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                triangleBarriarBuffer.SetData(triangles);
                
                var matrixBuffer = new ComputeBuffer(matrixList.Count, matrixStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                matrixBuffer.SetData(matrixList);

                //开始进行biarrierBuffer的更新
               
                cs.SetBuffer(kernel_computeBarrier, nameId_BarrierBuffer, barrierBuffer);
                cs.SetInts(nameId_BlockNum, new int[]
                {
                    blockNum.x, blockNum.y, blockNum.z
                });
                cs.SetVector(nameId_BlockSize, blockSize);
                cs.SetVector(nameId_Origin, origin);
                cs.SetBuffer(kernel_computeBarrier, nameId_VerticesBuffer, verticesBuffer);
                cs.SetBuffer(kernel_computeBarrier, nameId_IndicesBuffer, indicesBuffer);
                cs.SetBuffer(kernel_computeBarrier, nameId_MatrixBuffer, matrixBuffer);
                cs.SetBuffer(kernel_computeBarrier, nameId_TriangleBarrierBuffer, triangleBarriarBuffer);
                cs.SetInt(nameId_TriangleCount, triangleTotalCount);
               
                var group2 = GetGroup(triangleTotalCount);
                cs.Dispatch(kernel_computeBarrier, group2.x, group2.y, group2.z);
                
                matrixBuffer.Release();
                triangleBarriarBuffer.Release();
            }
            //设置灯光值

        }



        private void UpdateVerticesBuffer(IEnumerable<Mesh> meshs)
        {
            bool isHasNewMesh = false;
            foreach (var mesh in meshs)
            {
                if (meshStartIdDic.ContainsKey(mesh)) continue;
                isHasNewMesh = true;
                int vertexCountSum = 0;
                int indexCountSum = 0;
                foreach (var key in meshStartIdDic.Keys)
                {
                    vertexCountSum += key.vertexCount;
                    indexCountSum += (int)key.GetIndexCount(0);
                }
                meshStartIdDic[mesh] = new Vector2Int(vertexCountSum, indexCountSum);
            }
            if (!isHasNewMesh) return;
            var totalVertCount = 0;
            var totalIndexCount = 0;

            foreach (var key in meshStartIdDic.Keys)
            {
                totalVertCount += key.vertexCount;
                totalIndexCount += (int)key.GetIndexCount(0);
            }
            var totalVertices = new Vector3[totalVertCount];
            var totalIndices = new int[totalIndexCount];
            foreach (var kvp in meshStartIdDic)
            {
                var vertexStart = kvp.Value.x;
                var indexStart = kvp.Value.y;
                var mesh = kvp.Key;
                mesh.vertices.CopyTo(totalVertices, vertexStart);
                var indices = mesh.GetIndices(0);
                var indicesCount = indices.Length;
                for (int i = 0; i < indicesCount; i++)
                {
                    indices[i] += vertexStart;
                }
                indices.CopyTo(totalIndices, indexStart);
            }

            if (verticesBuffer != null) verticesBuffer.Release();
            verticesBuffer = new ComputeBuffer(totalVertCount, vertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            verticesBuffer.SetData(totalVertices);

            if(indicesBuffer != null) indicesBuffer.Release();
            indicesBuffer = new ComputeBuffer(totalIndexCount, vertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            indicesBuffer.SetData(totalIndices);

        }
    }

}

