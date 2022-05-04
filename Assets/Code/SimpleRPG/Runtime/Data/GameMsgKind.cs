namespace SimpleRPG.Runtime
{
    [XLua.LuaCallCSharp]
    public class GameMsgKind
    {
        private const int prefix = 1 << 8;
        //没有数据 一帧一次
        public const int Custom = prefix | 0;
        //移动执行
        public const int Move = prefix | 1;
        
        //镜头跟随
        public const int FollowTarget = prefix | 2;
        //普通攻击
        public const int Attack = prefix | 3;
        //收集敌人
        public const int CollectEnemy = prefix | 4;   
        //被击
        public const int Hit = prefix | 5;
        //颜色tween
        public const int ColorTween = prefix | 6;
        //收集世界状态相关 7 - 100
        public const int Collect_Launcher = prefix | 7;
        public const int Collect_InAttackRange = prefix | 8;
        public const int Collect_AttackCoolDown = prefix | 9;
        public const int Collect_Player = prefix | 10;
    }
}