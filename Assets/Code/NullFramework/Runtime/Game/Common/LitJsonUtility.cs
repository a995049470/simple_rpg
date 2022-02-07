using LitJson;
using UnityEngine;

namespace NullFramework.Runtime
{
    public static class LitJsonUtility
    {
        private static bool isLitJsonExpand = false;

        public static void LitJsonExpand()
        {
            if(isLitJsonExpand) return;
            isLitJsonExpand = true;
            JsonMapper.RegisterImporter<string, float>(str =>
            {
                return float.Parse(str);
            });

             JsonMapper.RegisterImporter<double, float>(d =>
            {
                return (float)d;
            });
            JsonMapper.RegisterExporter<System.Single>((f, writer) =>
            {
              
                writer.Write(f.ToString());
                
            });
            JsonMapper.RegisterExporter<Vector3>((v3, writer) => 
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("x");
                writer.Write(v3.x.ToString());
                writer.WritePropertyName("y");
                writer.Write(v3.y.ToString());
                writer.WritePropertyName("z");
                writer.Write(v3.z.ToString());
                writer.WriteObjectEnd();
            });


        }
    }
}