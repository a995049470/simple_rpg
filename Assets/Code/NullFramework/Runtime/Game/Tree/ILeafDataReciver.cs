namespace NullFramework.Runtime
{
    //叶数据接收者
    public interface ILeafDataReciver
    {
        void SetLeafData(LeafData data);
        //接收数据结束
        void OnReciveDataFinish();
    }
}

