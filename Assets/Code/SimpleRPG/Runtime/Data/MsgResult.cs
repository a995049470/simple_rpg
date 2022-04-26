namespace SimpleRPG.Runtime
{
    public enum MsgResult
    {
        //执行中
        Runing = 0,
        //无法释放
        Fail = 1,
        //攻击过程中被取消
        Cancel = 2,
        //成功执行
        Success = 3
    }

}
