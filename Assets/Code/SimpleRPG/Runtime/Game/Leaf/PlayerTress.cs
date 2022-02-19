using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class PlayerTress : Tress<PlayerTressData>, IUnityObjectLoader, IUnityObjectGetter
    {
        private GameObject player;

        public Object GetUnityObject()
        {
            return player;
        }

        public void LoadUnityObject()
        {
            player = data.InstantiatePlayer();
        }
        
    }
}


