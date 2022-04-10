using NullFramework.Runtime;
using UnityEngine;
namespace SimpleRPG.Runtime
{
    public class AbilityTressData : LeafData<AbilityTress>
    {
        [SerializeField]
        private int hp;
        public int Hp { get => hp; }
        [SerializeField]
        private int atk;
        public int Atk { get => atk; }
        
    }
}

