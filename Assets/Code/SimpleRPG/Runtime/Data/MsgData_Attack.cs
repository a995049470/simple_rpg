using System.Collections.Generic;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{

    //攻击命令只能被一个人接收
    public class MsgData_Attack : MsgData
    {
        public int attackerFilter;
        public BattleUnit attacker;
        public List<BattleUnit> hiters;
        public int attackState;

        
        // public MsgData_Attack Clone()
        // {
        //     var data = new MsgData_Attack();
        //     data.attacker = this.attacker;
        //     if(this.hiters != null) data.hiters = new List<BattleUnit>(this.hiters);
        //     return data;
        // }
    }

}
