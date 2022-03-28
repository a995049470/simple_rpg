using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class EnemyTress : Tress<EnemyTressData>
    {
        private GameObject enemy;
        public override void OnReciveDataFinish()
        {
            enemy = leafData.InstantiateEnemy();
        }
        
    }
}


