using UnityEngine;

namespace NullFramework.Runtime
{
    public class DestoryOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}