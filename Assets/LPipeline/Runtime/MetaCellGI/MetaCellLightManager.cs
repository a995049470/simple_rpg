using System.Collections.Generic;

namespace LPipeline.Runtime
{
    public class MetaCellLightManager : Single<MetaCellLightManager>
    {
        private List<MetaCellLight> lights = new List<MetaCellLight>();

        public List<MetaCellLight> Lights { get => lights; }

        public void Add(MetaCellLight light)
        {
            lights.Add(light);
        }

        public void Remove(MetaCellLight light)
        {
            lights.Remove(light);
        }
    }
}

