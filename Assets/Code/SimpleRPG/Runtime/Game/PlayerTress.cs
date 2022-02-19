using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class PlayerTress : Tress, IUnityObjectLoader, IUnityObjectGetter, ILeafDataSetter
    {
        private GameObject player;
        private PlayerTressData data;

        public Object GetUnityObject()
        {
            return player;
        }

        public void LoadUnityObject()
        {
            player = data.InstantiatePlayer();
        }

        public void SetLeafData(LeafData data)
        {
            this.data = data as PlayerTressData;
        }
    }
}


