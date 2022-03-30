

namespace NullFramework.Runtime
{
    public class ObjectArray : RingArray<object>
    {
        public ObjectArray(int _size) : base(_size)
        {

        }
        public void Free(int pos)
        {
        #if UNITY_EDITOR
            if(pos < 0 || pos >= size)
            {
                throw new System.Exception("无效的位置");
            }
        #endif
            values[pos] = null;
        }

        public override object Pop()
        {
            var obj = base.Pop();
        
            //填补中间的空位
            while (count > 0 && values[end] == null)
            {
                end = (end + size - 1) % size;
                count --;
            }
            return obj;
        }

        public override object Dequeue()
        {
            var obj = base.Dequeue();
            //填补中间的空位
            while (count > 0 && values[start] == null)
            {
                start = (start + 1) % size;
                count --;
            }
            return obj;
        }

    }
}