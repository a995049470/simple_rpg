using System.Collections.Generic;
using UnityEngine;

namespace LPipeline.Runtime
{
    public class MetaCellGIBarrierManager : Single<MetaCellGIBarrierManager>
    {
        
        private List<MetaCellBarrier> barriers = new List<MetaCellBarrier>();

        public void Add(MetaCellBarrier barrier)
        {
            barriers.Add(barrier);
        }

        public void Remove(MetaCellBarrier barrier)
        {
            barriers.Remove(barrier);
        }

        //获取障碍mesh  List<localToWorldMatrix>
        public Dictionary<Mesh, List<Matrix4x4>> GetBarrierDic()
        {
            var dic = new Dictionary<Mesh, List<Matrix4x4>>();
            foreach (var barrier in barriers)
            {
                var mesh = barrier.BarrierMesh;
                if(mesh == null) continue;
                List<Matrix4x4> list;
                if(!dic.TryGetValue(mesh, out list))
                {
                    list = new List<Matrix4x4>();
                    dic[mesh] = list;
                }
                var matrix = Matrix4x4.TRS(barrier.transform.position, barrier.transform.rotation, barrier.transform.lossyScale);
                // var matrix = barrier.transform.localToWorldMatrix;
                list.Add(matrix);
            }
            return dic;
        }
    }
}

