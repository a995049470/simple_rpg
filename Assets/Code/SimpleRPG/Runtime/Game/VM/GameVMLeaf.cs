using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class Attacker
    {
        private Leaf leaf;

    }

    public class GameVM : VM
    {   
        private static GameVM prefab;

        public GameVM() : base()
        {

        }

        public GameVM(VM vm) : base(vm)
        {

        }

        public static GameVM CreateVM()
        {
            if(prefab == null) prefab = new GameVM();
            return new GameVM(prefab);
        }
        
        //attack  radius count : 攻击者类
        protected void CreateAttacker()
        {

        }

        //攻击者类 : 攻击者 被攻击者
        protected void FindEnemy()
        {
            
        }

        // : 战斗进度
        protected void Progress()
        {
            
        }
        
        //攻击者 被攻击者 战斗进度 : 失败? 攻击者 被攻击者 战斗进度
        protected void CheckAttackComplete()
        {   

        }

    }
}