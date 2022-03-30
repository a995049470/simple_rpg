namespace SimpleRPG.Runtime
{
    public class GameMsgKind
    {
        private const int prefix = 1 << 8;
        //没有数据 一帧一次
        public const int custom = prefix | 0;
        //移动执行
        public const int Move = prefix | 1;
        
        public const int FollowTarget = prefix | 2;

        
    }

}