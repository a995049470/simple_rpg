using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class MsgData_CollectEnemy : MsgData
    {
        public float radius;
        public Vector3 center;
        private int attackNum;
        private int currentCount;
        public List<BattleUnit> hiters;
        public bool isCurrentEnemyInRange;

        public void SetAttackNum(int num)
        {
            attackNum = num;
            currentCount = 0;
        }
    
        public void TryAddEnemy(Leaf hiter, GameObject obj)
        {
            if(currentCount >= attackNum)
            {
                isCurrentEnemyInRange = false;
                return;
            }
            var position = obj.transform.position;
            var dis = Vector3.Distance(center, position);
            isCurrentEnemyInRange = dis < radius;
            if(isCurrentEnemyInRange)
            {
                var battleUnit = new BattleUnit();
                battleUnit.leaf = hiter;
                battleUnit.unitObj = obj;
                hiters.Add(battleUnit);
                currentCount ++;
            }
        }

        public void TryAddEnemyAblity(AbilityData abilityData)
        {
            if(!isCurrentEnemyInRange) return;
            hiters[currentCount - 1].abilityData = abilityData;
        }

    }

}

