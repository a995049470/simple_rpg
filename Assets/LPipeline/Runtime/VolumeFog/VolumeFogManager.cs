using System.Collections.Generic;

namespace LPipeline.Runtime
{
    public class VolumeFogManager : Single<VolumeFogManager>
    {
       private Dictionary<int, List<VolumeFog>> fogsDic = new Dictionary<int, List<VolumeFog>>();
       public Dictionary<int, List<VolumeFog>> FogsDic { get => fogsDic; }

        public void Remove(VolumeFog fog, int layer)
        {
            if(fogsDic.TryGetValue(layer, out var fogs))
            {
                fogs.Remove(fog);
            }
        }

        public void Add(VolumeFog fog, int layer)
        {
            if(!fogsDic.TryGetValue(layer, out var fogs))
            {
                fogs = new List<VolumeFog>();
                fogsDic[layer] = fogs;
            } 
            fogs.Add(fog);
        }

    }
}

