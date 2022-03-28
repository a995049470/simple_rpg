namespace NullFramework.Runtime
{
    public static partial class BaseMsgKind
    {
        private const int prefix = 0;
        public const int Update = prefix | 1;
        //data:GameObject
        public const int InitFightMap = prefix | 2;      
    }
}
