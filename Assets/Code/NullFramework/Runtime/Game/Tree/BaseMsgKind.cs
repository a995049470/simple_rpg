namespace NullFramework.Runtime
{
    public static class BaseMsgKind
    {
        private const int prefix = 0;
        public const int GoapUpdate = prefix | 1;
        //data:GameObject
        public const int InitFightMap = prefix | 2;  
        public const int BehaviorUpdate = prefix | 3;    
    }


    public static class StateKind
    {
        /// <summary>
        /// 未被初始化的状态
        /// </summary>
        public const int None = 0;
        //运行
        public const int Runing = 1;
        //失败
        public const int Fail = 2;
        //被取消
        public const int Cancel = 3;
        //成功
        public const int Success = 4; 
    }
    
}
