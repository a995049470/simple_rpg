namespace SimpleRPG.Runtime
{
    //战斗单位类
    public class BattleUnit : BaseUnit
    {
        public AbilityData abilityData;
        public int attackFilter = -1;
        public int unitKind;
        //距离攻击者的距离
        public float distance;
    }

}
