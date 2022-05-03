namespace SimpleRPG.Runtime
{
    public enum GameStateType
    {
        //发起者 BaseUnit
        Launcher = 0,
        //在攻击范围内 bool
        InAttackRange = 1,
        //攻击冷却完毕 bool
        AttackCoolDown = 2,
        //攻击对象 BaseUnit
        AttackTarget = 3,
        
    }
}
