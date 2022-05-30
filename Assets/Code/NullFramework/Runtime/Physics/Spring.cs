namespace NullFramework.Runtime
{
    public struct Spring
    {
        //两端
        public int i;
        public int j;
        //原长
        public float length;

        public Spring(int _i, int _j, float _len)
        {
            i = _i;
            j = _j;
            length = _len;
        }
    }
}

