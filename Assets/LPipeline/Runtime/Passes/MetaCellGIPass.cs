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

        //渲染场景的diffuse
        [SerializeField]
        private Material diffuseMaterial;

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
        
        [SerializeField][Range(1, 16)]
        private int iteratCount;


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
            lightColorTexture = RenderTexture.GetTemporary(des);
            globalLightColorBackTexture = RenderTexture.GetTemporary(des);
            globalLightColorFrontTexture = RenderTexture.GetTemporary(des);
            globalLightColorBackTexture.Create();
            globalLightColorFrontTexture.Create();
            lightColorTexture.Create();
            globalLightColorBackTexture.filterMode = FilterMode.Trilinear;
            globalLightColorFrontTexture.filterMode = FilterMode.Trilinear;

        }

        public override void FirstCall()
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
                RenderTexture.ReleaseTemporary(lightColorTexture);
                RenderTexture.ReleaseTemporary(globalLightColorBackTexture);
                RenderTexture.ReleaseTemporary(globalLightColorFrontTexture);
                CreateLightTexture();
            }
        }


        public override void EndCall()
        {
            RenderTexture.ReleaseTemporary(lightColorTexture);
            RenderTexture.ReleaseTemporary(globalLightColorBackTexture);
            RenderTexture.ReleaseTemporary(globalLightColorFrontTexture);
            barrierBuffer.Release();
            indicesBuffer.Release();
            verticesBuffer.Release();
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
            return cs != null && diffuseMaterial != null;
        }

        private void Dispatch_ClearBarrier(CommandBuffer cmd)
        {
            cmd.SetComputeBufferParam(cs, kernel_ClearBarrier, ShaderUtils._BarrierBuffer, barrierBuffer);
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
                    triangleBarriarBuffer = new ComputeBuffer((int)triangleTotalCount, TriangleBarrier.Size);

                }
                triangleBarriarBuffer.SetData(triangles);
                
                if(sum_matrixCount != matrixBuffer.count)
                {
                    matrixBuffer.Release();
                    matrixBuffer = new ComputeBuffer(sum_matrixCount, matrixStride);
                }
                matrixBuffer.SetData(matrixList);

                //开始进行biarrierBuffer的更新

                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, ShaderUtils._BarrierBuffer, barrierBuffer);
                cmd.SetComputeIntParams(cs, ShaderUtils._BlockNum, new int[]
                {
                    blockNum_3d.x, blockNum_3d.y, blockNum_3d.z
                });
                cmd.SetComputeVectorParam(cs, ShaderUtils._BlockSize, blockSize);
                cmd.SetComputeVectorParam(cs, ShaderUtils._Origin, origin);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, ShaderUtils._VerticesBuffer, verticesBuffer);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, ShaderUtils._IndicesBuffer, indicesBuffer);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, ShaderUtils._MatrixBuffer, matrixBuffer);
                cmd.SetComputeBufferParam(cs, kernel_ComputeBarrier, ShaderUtils._TriangleBarrierBuffer, triangleBarriarBuffer);
                cmd.SetComputeIntParam(cs, ShaderUtils._TriangleCount, triangleTotalCount);
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
                cmd.SetComputeIntParams(cs, ShaderUtils._BlockNum, new int[]
                {
                    blockNum_3d.x, blockNum_3d.y, blockNum_3d.z
                });
                cmd.SetComputeVectorParam(cs, ShaderUtils._Origin, origin);
                cmd.SetComputeVectorParam(cs, ShaderUtils._BlockSize, blockSize);
                cmd.SetComputeBufferParam(cs, kernel_FillLight, ShaderUtils._LightBuffer, lightBuffer);
                cmd.SetComputeIntParam(cs, ShaderUtils._LightCount, lightCount);
                cmd.SetComputeTextureParam(cs, kernel_FillLight, ShaderUtils._GlobalLightColorFrontTexture, globalLightColorFrontTexture);

                var group = GetGroup(lightCount);
                cmd.DispatchCompute(cs, kernel_FillLight, group.x, group.y, group.z);
            }
        }

        private void Dispatch_UpdateGlobalLightColor(CommandBuffer cmd)
        {
            cmd.SetComputeTextureParam(cs, kernel_UpdateGlobalLightColor, ShaderUtils._GlobalLightColorBackTexture, globalLightColorBackTexture);
            cmd.SetComputeTextureParam(cs, kernel_UpdateGlobalLightColor, ShaderUtils._GlobalLightColorFrontTexture, globalLightColorFrontTexture);
            cmd.SetComputeBufferParam(cs, kernel_UpdateGlobalLightColor, ShaderUtils._BarrierBuffer, barrierBuffer);
            cmd.SetComputeIntParams(cs, ShaderUtils._BlockNum, new int[]
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
            SetDefaultRenderTarget(cmd, context, data);
            //清理障碍值
            Dispatch_ClearBarrier(cmd);
            // //计算障碍
            Dispatch_ComputeBarrier(cmd);    
            
            //填充灯光
            //更新全局灯光
            for (int i = 0; i < iteratCount; i++)
            {
                Dispatch_FillLight(cmd);
                Dispatch_UpdateGlobalLightColor(cmd);
            }
            
            
            //更新光照贴图
            cmd.SetGlobalTexture(ShaderUtils._GlobalLightColorTexture, globalLightColorFrontTexture);
            cmd.SetGlobalVector(ShaderUtils._Origin, origin);
            cmd.SetGlobalVector(ShaderUtils._BlockNum, (Vector3)blockNum_3d);
            cmd.SetGlobalVector(ShaderUtils._BlockSize, blockSize);
            cmd.DrawMesh(GetFullScreenQuad(), Matrix4x4.identity, diffuseMaterial);

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

