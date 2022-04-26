using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class BattleUnit
    {
        public Leaf leaf;
        public GameObject unitObj;
        public AbilityData abilityData;
        public int attackFilter = -1;
        public int unitKind;
        //距离攻击者的距离
        public float distance;
        
    
    }

}
