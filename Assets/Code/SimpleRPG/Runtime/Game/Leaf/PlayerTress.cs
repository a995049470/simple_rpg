using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class PlayerTress : Tress<PlayerTressData>
    {
        private GameObject player;
        public GameObject Player { get => player; }

    
        public override void OnReciveDataFinish()
        {
            player = leafData.InstantiatePlayer();
        }
    }
}


