using System;

namespace NullFramework.Runtime
{

    public abstract class Single<T> where T :  class, IDisposable, new()
    {
        
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if(s_instance == null)
                {
                    s_instance = new T();
                }
                return s_instance;
            }
        }
        public static void Clear()
        {
            s_instance?.Dispose();
            s_instance = null;
        }
        
        
    }
    
}
