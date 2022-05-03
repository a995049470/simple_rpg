namespace NullFramework.Runtime
{
    /// <summary>
    /// 运行的状态描述
    /// </summary>
    public static class RunState
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
