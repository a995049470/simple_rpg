using System.Collections.Generic;
using UnityEngine;
using NullFramework.Runtime;
using System;

namespace SimpleRPG.Runtime
{
    public class CubeRendererCenter : Single<CubeRendererCenter>, IDisposable
    {
        private List<CubeRenderer> cubeRenderers = new List<CubeRenderer>();
        //private ComputeBuffer cubeBuffers;

        public void Add(CubeRenderer renderer)
        {
            cubeRenderers.Add(renderer);
        }

        public void Remove(CubeRenderer renderer)
        {
            cubeRenderers.Remove(renderer);
        }

        public void Refresh()
        {
            foreach (var cubeRenderer in cubeRenderers)
            {
                cubeRenderer.InitBuffer();
            }
        }

        public void GetBufferData(out ComputeBuffer cubeGBuffer, out int instanceCount)
        {
            
            int num = 0;
            foreach (var renderer in cubeRenderers)
            {
                num += renderer.GetCubeCount();
            }
            instanceCount = num;
            if(num == 0)
            {
                cubeGBuffer = null;
                return;
            }
            
            cubeGBuffer = new ComputeBuffer(num, CubeGBuffer.Size);
            int start = 0;
            foreach (var renderer in cubeRenderers)
            {
                var buffer = renderer.GetCubeGBuffer();
                var size = buffer.Length;
                cubeGBuffer.SetData(buffer, 0, start, size);
                start += size;
            }
        }

        public void Dispose()
        {
            
        }
    }

}
