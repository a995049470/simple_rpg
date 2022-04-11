using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{

    [CreateAssetMenu(fileName = "AbilityTressData", menuName = "SimpleRPG/AbilityTressData")]
    public class AbilityTressData : LeafData<AbilityTress>
    {

        public int hp;
        public int atk;
        
    }
}

