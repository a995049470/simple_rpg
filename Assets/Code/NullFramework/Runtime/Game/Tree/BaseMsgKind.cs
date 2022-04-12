namespace NullFramework.Runtime
{
    public static partial class BaseMsgKind
    {
        private const int prefix = 0;
        public const int GoapUpdate = prefix | 1;
        //data:GameObject
        public const int InitFightMap = prefix | 2;  
        public const int BehaviorUpdate = prefix | 3;    
        public const int CollectChilds = prefix | 4;
    }

    
}
