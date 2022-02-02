namespace NullFramework.Runtime
{
    public interface IHandle
    {
        void SetHandle(int handle);
        int GetHandle_I32();
        void OnFree();
    }

}