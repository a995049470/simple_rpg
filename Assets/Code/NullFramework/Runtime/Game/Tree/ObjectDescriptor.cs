namespace NullFramework.Runtime
{
    public class ObjectDescriptor
    {
        public object obj;
        public int kind;
        public ObjectDescriptor(int _kind = 0, object _obj = null)
        {
            obj = _obj;
            kind = _kind;
        }
    }
}
