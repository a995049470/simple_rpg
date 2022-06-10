using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LPipeline.Runtime
{
    public struct TriangleBarrier
    {
        public int i0Id;
        public int matrixId;

        public const int Size = 2 * sizeof(uint);
    }

    [CreateAssetMenu(fileName = "MetaCellGIPass", menuName = "LPipeline/Passes/MetaCellGIPass")]
    public class MetaCellGIPass : RenderPass
    {
        //范围
        [SerializeField]
        private Vector3 xyzMin = new Vector3(-16, -16, -16);
        [SerializeField]
        private Vector3 xyzMax = new Vector3(16, 16, 16);

        //障碍相关
        private ComputeBuffer barrierBuffer;
        private Vector3Int barrierBufferSize = new Vector3Int(128, 128, 128);
        private int barrierBufferCount { get => barrierBufferSize.x * barrierBufferSize.y * barrierBufferSize.z; }
        private const int barrierStride = 3 * sizeof(float);

        //把所有障碍物分解成三角形
        private Dictionary<Mesh, Vector2Int> meshStartIdDic;
        private ComputeBuffer verticesBuffer;
        private ComputeBuffer indicesBuffer;
        private const int vertStride = 3 * sizeof(float);
        private const int matrixStride = 16 * sizeof(float);

        //灯光亮度相关
        
        //计算着色器相关
        private ComputeShader computeShader;
        private int kernel_computeBarrier;
        private string name_computeBarrier;
        private int kernel_clearBarrier;
        private string name_clearBarrier;




        private void OnEnable()
        {
            meshStartIdDic = new Dictionary<Mesh, Vector2Int>();
            barrierBuffer = new ComputeBuffer(barrierBufferCount, barrierStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            verticesBuffer = new ComputeBuffer(1, vertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            indicesBuffer = new ComputeBuffer(1, vertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            // kernel_computeBarrier = computeShader.FindKernel(name_computeBarrier);
            // kernel_clearBarrier = computeShader.FindKernel(name_clearBarrier);
    
        }

        private void OnDisable()
        {
            barrierBuffer?.Release();
            indicesBuffer?.Release();
            verticesBuffer?.Release();
        }

        public override void Execute(ScriptableRenderContext context, RenderData data)
        {
            //准备computeBuffer使用的数据
            var barrierDic = MetaCellGIBarrierManager.Instance.GetBarrierDic();
            UpdateVerticesBuffer(barrierDic.Keys);
            int triangleTotalCount = 0;
            foreach (var kvp in barrierDic)
            {
                triangleTotalCount += (int)kvp.Key.GetIndexCount(0) / 3 * kvp.Value.Count;
            }

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
                            triangles[triangleId].i0Id = indexStartId + j * 3;
                            triangles[triangleId].matrixId = sum_matrixCount;
                        }      
                        sum_triangleCount += triangleCount;
                        sum_matrixCount += 1;              
                    }
                }
                
                var triangleBarriarBuffer = new ComputeBuffer((int)triangleTotalCount, TriangleBarrier.Size, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                triangleBarriarBuffer.SetData(triangles);
                
                var matrixBuffer = new ComputeBuffer(matrixList.Count, matrixStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
                matrixBuffer.SetData(matrixList);
                matrixBuffer.Release();
                triangleBarriarBuffer.Release();
            }

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

