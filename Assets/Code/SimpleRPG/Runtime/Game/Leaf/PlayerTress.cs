using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class PlayerTress : Tress<PlayerTressData>, IUnityObjectLoder, ILeafSender
    {
        private GameObject player;
        public GameObject Player { get => player; }

        

        public void LoadUnityObject()
        {
            player = data.InstantiatePlayer();
        }

        public Leaf SendLeaf()
        {
            return this;
        }
    }
}


