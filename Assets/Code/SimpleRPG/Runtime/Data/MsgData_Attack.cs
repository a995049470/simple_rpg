using System.Collections.Generic;
using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{

    public class MsgData_Attack : MsgData
    {
        public Tress attacker;   
        public AbilityData attackAbilityData;

        public List<Tress> hiters = new List<Tress>();
        public List<AbilityData> hiterAbilityDatas = new List<AbilityData>();

        public MsgData_Attack Clone()
        {
            var data = new MsgData_Attack();
            data.attacker = this.attacker;
            data.attackAbilityData = this.attackAbilityData;
            data.hiters = new List<Tress>(this.hiters);
            data.hiterAbilityDatas = new List<AbilityData>(this.hiterAbilityDatas);
            return data;
        }
    }

}
