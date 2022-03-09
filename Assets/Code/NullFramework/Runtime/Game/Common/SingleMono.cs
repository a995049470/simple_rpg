using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{
    public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if(s_instance == null)
                {
                    new GameObject(typeof(T).Name, typeof(T));
                }
                return s_instance;
            }
        }

        protected virtual void Awake()
        {
            if(s_instance != null)
            {
                Destroy(this.gameObject.GetComponent<T>());
            }
            else
            {
                s_instance = this.GetComponent<T>();
                if(Application.isPlaying)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }
    
}

