using System.Collections.Generic;
using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class MsgData_CollectEnemy : MsgData
    {
        public float radius;
        private Leaf attacker;
        public Vector3 center;
        private int attackNum;
        private int currentEnemyId;
        //过滤敌人的类型
        public int attackFilter = -1;
        public List<BattleUnit> hiters;

        public void SetAttackNum(int num)
        {
            attackNum = num;
            //currentCount = 0;
            currentEnemyId = -1;
           
        }
    
        public void TryAddEnemy(Leaf hiter, GameObject obj, int unitKind)
        {
        
            currentEnemyId = -1;
            //攻击者的类型过滤
            if((unitKind & attackFilter) == 0)
            {
                return;
            }
            var position = obj.transform.position;
            var dis = Vector3.Distance(center, position);
            var isCurrentEnemyInRange = dis < radius;
            if(isCurrentEnemyInRange)
            {
                var battleUnit = new BattleUnit();
                battleUnit.leaf = hiter;
                battleUnit.unitObj = obj;
                battleUnit.distance = dis;
                int id;
                for (id = hiters.Count ; id >= 1; id --)
                {
                    var lastHiter = hiters[id - 1];
                    if(dis > lastHiter.distance)
                    {
                        break;
                    }
                }
                if(id < hiters.Count)
                {
                    //满了 移除最后一个
                    if(hiters.Count == attackNum)
                    {
                        hiters.RemoveAt(hiters.Count - 1);
                    }
                    hiters.Insert(id, battleUnit);
                    currentEnemyId = id;
                    
                }
                else if(id < attackNum)
                {
                    hiters.Add(battleUnit);
                    currentEnemyId = id;
                }
            }
        }

        public void TryAddEnemyAblity(AbilityData abilityData)
        {
            if(currentEnemyId < 0) return;
            hiters[currentEnemyId].abilityData = abilityData;
        }

    }

}

