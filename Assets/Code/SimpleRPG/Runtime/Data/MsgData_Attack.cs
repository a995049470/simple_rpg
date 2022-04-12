using System.Collections.Generic;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class MsgData_Attack : MsgData
    {
        public bool isPlayer;
        public BattleUnit attacker;
        public List<BattleUnit> hiters;


        public MsgData_Attack Clone()
        {
            
            var data = new MsgData_Attack();
            data.isPlayer = this.isPlayer;
            data.attacker = this.attacker;
            if(this.hiters != null) data.hiters = new List<BattleUnit>(this.hiters);
            return data;
        }
    }

}
