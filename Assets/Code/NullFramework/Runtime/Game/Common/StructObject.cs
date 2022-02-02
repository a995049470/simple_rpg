namespace NullFramework.Runtime
{
    public class SturctObject<T> where T : struct
    {
        public T Value;
        public SturctObject(T value)
        {
            this.Value = value;
        }
    }
}