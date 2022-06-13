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

    public struct CellLight
    {
        public Vector4 position;
        public Vector4 color;
        public const int Size = 8 * sizeof(float);
    }

    [CreateAssetMenu(fileName = "MetaCellGIPass", menuName = "LPipeline/Passes/MetaCellGIPass")]
    public class MetaCellGIPass : RenderPass
    {
        private bool isDity = true;
        private bool isInit = false;
        //范围
        [SerializeField]
        private Vector3 origin = new Vector3(-32, -32, -32);
        [SerializeField]
        private Vector3 blockSize = new Vector3(0.5f, 0.5f, 0.5f);

        //障碍相关
        private ComputeBuffer barrierBuffer;
        [SerializeField]
        private Vector3Int blockNum_3d = new Vector3Int(128, 128, 128);
        private int blockCount { get => blockNum_3d.x * blockNum_3d.y * blockNum_3d.z; }
        private const int barrierStride = 3 * sizeof(uint);

        //把所有障碍物分解成三角形
        private Dictionary<Mesh, Vector2Int> meshStartIdDic;
        private ComputeBuffer verticesBuffer;
        private ComputeBuffer indicesBuffer;
        private ComputeBuffer matrixBuffer;
        private ComputeBuffer triangleBarriarBuffer;
        private const int vertStride = 3 * sizeof(float);
        private const int matrixStride = 16 * sizeof(float);

        //灯光亮度相关
        private ComputeBuffer lightBuffer;
        private RenderTexture lightColorTexture;
        private RenderTexture globalLightColorFrontTexture;
        private RenderTexture globalLightColorBackTexture;
        


        //计算着色器相关
        [SerializeField]
        private ComputeShader cs;
        private int kernel_ComputeBarrier;
        private const string name_computeBarrier = "ComputeBarrier";
        private int kernel_ClearBarrier;
        private const string name_ClearBarrier = "ClearBarrier";
        private int kernel_ClearLightColor;
        private const string name_ClearLightColor = "ClearLightColor";
        private int kernel_FillLight;
        private const string name_FillLight = "FillLight";
        private int kernel_UpdateGlobalLightColor;
        private const string name_UpdateGlobalLightColor = "UpdateGlobalLightColor";
        
        private const int NumThreadX_1D = 64;
        private const int NumThreadY_1D = 1;
        private const int NumThreadZ_1D = 1;

        private const int NumThreadX_3D = 4;
        private const int NumThreadY_3D = 4;
        private const int NumThreadZ_3D = 4;
        private static int nameId_BarrierBuffer = Shader.PropertyToID("_BarrierBuffer");
        private static int nameId_BlockNum = Shader.PropertyToID("_BlockNum");
        private static int nameId_BlockSize = Shader.PropertyToID("_BlockSize");
        private static int nameId_Origin = Shader.PropertyToID("_Origin");
        private static int nameId_VerticesBuffer = Shader.PropertyToID("_VerticesBuffer");
        private static int nameId_IndicesBuffer = Shader.PropertyToID("_IndicesBuffer");
        private static int nameId_MatrixBuffer = Shader.PropertyToID("_MatrixBuffer");
        private static int nameId_TriangleBarrierBuffer = Shader.PropertyToID("_TriangleBarrierBuffer");
        private static int nameId_TriangleCount = Shader.PropertyToID("_TriangleCount");
        private static int nameId_LightBuffer = Shader.PropertyToID("_LightBuffer");
        private static int nameId_LightCount = Shader.PropertyToID("_LightCount");
        private static int nameId_LightColorTexture = Shader.PropertyToID("_LightColorTexture");
        private static int nameId_GlobalLightColorFrontTexture = Shader.PropertyToID("_GlobalLightColorFrontTexture");
        private static int nameId_GlobalLightColorBackTexture = Shader.PropertyToID("_GlobalLightColorBackTexture");
        
        private static int nameId_GlobalLightColorTexture = Shader.PropertyToID("_GlobalLightColorTexture");


        private void FindKernels()
        {
            kernel_ComputeBarrier = cs.FindKernel(name_computeBarrier);
            kernel_ClearBarrier = cs.FindKernel(name_ClearBarrier);
            kernel_ClearLightColor = cs.FindKernel(name_ClearLightColor);
            kernel_FillLight = cs.FindKernel(name_FillLight);
            kernel_UpdateGlobalLightColor = cs.FindKernel(name_UpdateGlobalLightColor);
        }

        private void CreateLightTexture()
        {
            var des = new RenderTextureDescriptor(blockNum_3d.x, blockNum_3d.y, RenderTextureFormat.ARGBFloat);
            des.dimension = TextureDimension.Tex3D;
            des.enableRandomWrite = true;
            des.volumeDepth = blockNum_3d.z;
            lightColorTexture = new RenderTexture(des);
            globalLightColorBackTexture = new RenderTexture(des);
            globalLightColorFrontTexture = new RenderTexture(des);
            globalLightColorBackTexture.filterMode = FilterMode.Trilinear;
            globalLightColorFrontTexture.filterMode = FilterMode.Trilinear;
            globalLightColorBackTexture.Create();
            globalLightColorFrontTexture.Create();lightColorTexture.Create();
        }

        private void OnEnable()
        {
           
            {
                meshStartIdDic = new Dictionary<Mesh, Vector2Int>();

                verticesBuffer = new ComputeBuffer(1, vertStride);
                indicesBuffer = new ComputeBuffer(1, vertStride);

                triangleBarriarBuffer = new ComputeBuffer(1, TriangleBarrier.Size);

                matrixBuffer = new ComputeBuffer(1, matrixStride);
            }

            {
                barrierBuffer = new ComputeBuffer(blockCount, barrierStride);
            }

            if (cs != null)
            {
                FindKernels();
            }

            {
                
                CreateLightTexture();

                lightBuffer = new ComputeBuffer(1, CellLight.Size);
            }
        }


        private void OnValidate()
        {
            isDity = true;
        }

        private void Refresh()
        {
            if (!isDity) return;
            isDity = false;
            var currentBufferCount = barrierBuffer.count;
            if (currentBufferCount != blockCount)
            {
                barrierBuffer.Release();
                barrierBuffer = new ComputeBuffer(blockCount, barrierStride);
            }

            if (cs != null)
            {
                FindKernels();
            }

            var colorBufferNumX = globalLightColorBackTexture.width;
            var colorBufferNumY = globalLightColorBackTexture.height;
            var colorBufferNumZ = globalLightColorBackTexture.volumeDepth;
            if (colorBufferNumX != blockNum_3d.x ||
                colorBufferNumY != blockNum_3d.y ||
                colorBufferNumZ != blockNum_3d.z)
            {
                lightColorTexture.Release();
                globalLightColorBackTexture.Release();
                globalLightColorFrontTexture.Release();

                CreateLightTexture();
            }
        }


        private void OnDisable()
        {
            barrierBuffer.Release();
            indicesBuffer.Release();
            verticesBuffer.Release();
            lightColorTexture.Release();
            globalLightColorBackTexture.Release();
            globalLightColorFrontTexture.Release();
            matrixBuffer.Release();
            triangleBarriarBuffer.Release();
        }

        private Vector3Int GetGroup(int num)
        {
            int gx = Mathf.CeilToInt((float)num / NumThreadX_1D);
            return new Vector3Int(gx, 1, 1);
        }

        private Vector3Int GetGroup(Vector3Int num)
        {
            int gx = Mathf.CeilToInt((float)num.x / NumThreadX_3D);
            int gy = Mathf.CeilToInt((float)num.y / NumThreadY_3D);
            int gz = Mathf.CeilToInt((float)num.z / NumThreadZ_3D);
            return new Vector3Int(gx, gy, gz);
        }

        private bool Executable(){
            return cs != null;
        }

        private void Dispatch_ClearBarrier(CommandBuffer cmd)
        {
            cmd.SetComputeBufferParam(cs, kernel_ClearBarrier, nameId_BarrierBuffer, barrierBuffer);
            var group1 = GetGroup(blockCount);
            cmd.DispatchCompute(cs, kernel_ClearBarrier, group1.x, group1.y, group1.z);
        }

        private void Dispatch_ComputeBarrier(CommandBuffer cmd)
        {
            var barrierDic = MetaCellGIBarrierManager.Instance.GetBarrierDic();
            UpdateVerticesBuffer(barrierDic.Keys);
            int triangleTotalCount = 0;
            foreach (var kvp in barrierDic)
            {
                triangleTotalCount += (int)kvp.Key.GetIndexCount(0) / 3 * kvp.Value.Count;
            }

            //设置障碍值
            if (triangleTotalCount > 0)
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
                if(triangleTotalCount != triangleBarriarBuffer.count)
                {
                    triangleBarriarBuffer.Release();
                    triangleBarriarBuffer = new ComputeBuffer((int)triangleTotalCount, TriangleBarrier.Size, ComputeBufferType.Structured, ComputeBufferMode.Dynamic);

                }
                triangleBarriarBuffer.SetData(triangles);
                
                if(sum_matrixCount != matrixBuffer.count)
                {
                    matrixBuffer.Release();
                    matrixBuffer = new ComputeBuffer(sum_matrixCount, matrixStride, ComputeBufferType.Structured, ComputeBufferMode.Dynamic);
                }
                matrixBuffer.SetData(matrixList);

                //开始进行biarrierBuffer的更新

                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, nameId_BarrierBuffer, barrierBuffer);
                cmd.SetComputeIntParams(cs, nameId_BlockNum, new int[]
                {
                    blockNum_3d.x, blockNum_3d.y, blockNum_3d.z
                });
                cmd.SetComputeVectorParam(cs, nameId_BlockSize, blockSize);
                cmd.SetComputeVectorParam(cs, nameId_Origin, origin);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, nameId_VerticesBuffer, verticesBuffer);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, nameId_IndicesBuffer, indicesBuffer);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, nameId_MatrixBuffer, matrixBuffer);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, nameId_TriangleBarrierBuffer, triangleBarriarBuffer);
                cmd.SetComputeIntParam(cs, nameId_TriangleCount, triangleTotalCount);

                var group2 = GetGroup(triangleTotalCount);
                cmd.DispatchCompute(cs, kernel_ComputeBarrier, group2.x, group2.y, group2.z);

                
            }
        }

        private void Dispatch_FillLight(CommandBuffer cmd)
        {
            var lights = MetaCellLightManager.Instance.Lights;
            var lightCount = lights.Count;
            if(lightCount > 0)
            {
                var cellLights = new CellLight[lightCount];
                for (int i = 0; i < lightCount; i++)
                {
                    cellLights[i].position = lights[i].Position;
                    cellLights[i].color = lights[i].LightColor;
                }
                if(lightCount != lightBuffer.count)
                {
                    lightBuffer.Release();
                    lightBuffer = new ComputeBuffer(lightCount, CellLight.Size);
                }
                lightBuffer.SetData(cellLights);
                cmd.SetComputeIntParams(cs, nameId_BlockNum, new int[]
                {
                    blockNum_3d.x, blockNum_3d.y, blockNum_3d.z
                });
                cmd.SetComputeVectorParam(cs, nameId_Origin, origin);
                cmd.SetComputeVectorParam(cs, nameId_BlockSize, blockSize);
                cmd.SetComputeBufferParam(cs, kernel_FillLight, nameId_LightBuffer, lightBuffer);
                cmd.SetComputeIntParam(cs, nameId_LightCount, lightCount);
                cmd.SetComputeTextureParam(cs, kernel_FillLight, nameId_GlobalLightColorFrontTexture, globalLightColorFrontTexture);

                var group = GetGroup(lightCount);
                cmd.DispatchCompute(cs, kernel_FillLight, group.x, group.y, group.z);
            }
        }

        private void Dispatch_UpdateGlobalLightColor(CommandBuffer cmd)
        {
            cmd.SetComputeTextureParam(cs, kernel_UpdateGlobalLightColor, nameId_GlobalLightColorBackTexture, globalLightColorBackTexture);
            cmd.SetComputeTextureParam(cs, kernel_UpdateGlobalLightColor, nameId_GlobalLightColorFrontTexture, globalLightColorFrontTexture);
            cmd.SetComputeBufferParam(cs, kernel_UpdateGlobalLightColor, nameId_BarrierBuffer, barrierBuffer);
            cmd.SetComputeIntParams(cs, nameId_BlockNum, new int[]
                {
                    blockNum_3d.x, blockNum_3d.y, blockNum_3d.z
                });
            var group = GetGroup(blockNum_3d);
            cmd.DispatchCompute(cs, kernel_UpdateGlobalLightColor, group.x, group.y, group.z);
            //交换前后缓存
            SwapGlobalLightColorTexture();
        }

        private void SwapGlobalLightColorTexture()
        {
            var temp = globalLightColorFrontTexture;
            globalLightColorFrontTexture = globalLightColorBackTexture;
            globalLightColorBackTexture = temp;
        }
        
        
        public override void Execute(ScriptableRenderContext context, RenderData data)
        { 
            if(!Executable()) return;
            Refresh();
            var cmd = CommandBufferPool.Get(nameof(MetaCellGIPass));
            //清理障碍值
            Dispatch_ClearBarrier(cmd);
            // //计算障碍
            Dispatch_ComputeBarrier(cmd);            
            // //填充灯光
            Dispatch_FillLight(cmd);
            //更新全局灯光
            Dispatch_UpdateGlobalLightColor(cmd);
            
            //更新光照贴图
            cmd.SetGlobalTexture(nameId_GlobalLightColorTexture, globalLightColorFrontTexture);
            cmd.SetGlobalVector(nameId_Origin, origin);
            cmd.SetGlobalVector(nameId_BlockNum, (Vector3)blockNum_3d);
            cmd.SetGlobalVector(nameId_BlockSize, blockSize);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
            
            
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
            verticesBuffer = new ComputeBuffer(totalVertCount, vertStride);
            verticesBuffer.SetData(totalVertices);

            if (indicesBuffer != null) indicesBuffer.Release();
            indicesBuffer = new ComputeBuffer(totalIndexCount, vertStride);
            indicesBuffer.SetData(totalIndices);

        }
    }

}

