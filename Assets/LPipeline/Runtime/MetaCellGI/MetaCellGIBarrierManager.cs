using System.Collections.Generic;
using UnityEngine;

namespace LPipeline.Runtime
{
    public class MetaCellGIBarrierManager 
    {
        private static MetaCellGIBarrierManager instance;
        public static MetaCellGIBarrierManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new MetaCellGIBarrierManager();
                }
                return instance;
            }
        }
        private List<MetaCellGIBarrier> barriers = new List<MetaCellGIBarrier>();

        public void Add(MetaCellGIBarrier barrier)
        {
            barriers.Add(barrier);
        }

        public void Remove(MetaCellGIBarrier barrier)
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
                var matrix = barrier.transform.localToWorldMatrix;
                list.Add(matrix);
            }
            return dic;
        }
    }
}

