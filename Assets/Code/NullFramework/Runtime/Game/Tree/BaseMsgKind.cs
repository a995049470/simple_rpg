namespace NullFramework.Runtime
{
    public static class BaseMsgKind
    {
        private const int prefix = 0;
        public const int GoapUpdate = prefix | 1;
        //data:GameObject
        public const int InitFightMap = prefix | 2;  
        public const int BehaviorUpdate = prefix | 3;    
        //收集goapAction
        //public const int CollectGoapAction = prefix | 7; 
        //收集goapGoal
        public const int CollectGoapGoal = prefix | 8;
        public const int CollectWorldState = prefix | 9;

        //类实例化
        public const int ObjectInstantiate = prefix | 10;
        
        public const int ObjectDescriptorsUpdate = prefix | 11;
        
        public const int RigidUpdate = prefix | 12;
    }
    
}
