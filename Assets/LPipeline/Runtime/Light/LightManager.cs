using System.Collections.Generic;

namespace LPipeline
{
    public class LightManager
    {
        private static LightManager instance;
        public static LightManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new LightManager();
                }
                return instance;
            }
        }

        private List<Light_Point> pointLights = new List<Light_Point>();
        private List<Light_Directional> directionalLights = new List<Light_Directional>();
       

        public void AddLight(Light_Point light)
        {
            pointLights.Add(light);
        }

        public void RemoveLight(Light_Point light)
        {
            pointLights.Remove(light);
        }

        public void AddLight(Light_Directional light)
        {
            directionalLights.Add(light);
        }

        public void RemoveLight(Light_Directional light)
        {
            directionalLights.Remove(light);
        }

        //暂时拿最后一个平行光当阴影光源
        public Light_Directional GetShadowLight()
        {
            if(directionalLights.Count > 0)
            {
                return directionalLights[directionalLights.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// 获取所有可见的光源 以后考虑剔除...
        /// </summary>
        /// <returns></returns>
        public List<Light_Point> GetVisiblePointLights()
        {
            return pointLights;
        }

        public List<Light_Directional> GetVisibleDirectionalLights()
        {
            return directionalLights;
        }

        
    }
}

