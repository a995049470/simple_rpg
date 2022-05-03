namespace SimpleRPG.Runtime
{
    public enum GameStateType
    {
        //0不要用 被Target占用
        
        //在攻击范围内
        InAttackRange = 1,
        //攻击冷却完毕
        AttackCoolDown = 2,
        //攻击对象
        AttackTarget = 3,
    }

    // public static class GameStateKind
    // {
         
    // }
    
}
