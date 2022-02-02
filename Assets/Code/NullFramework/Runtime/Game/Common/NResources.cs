using UnityEngine;
namespace NullFramework.Runtime
{
    public class NResources {
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }
    }
}