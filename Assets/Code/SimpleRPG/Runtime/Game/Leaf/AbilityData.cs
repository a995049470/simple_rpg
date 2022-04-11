using NullFramework.Runtime;

namespace SimpleRPG.Runtime
{
    public class AbilityData
    {
        private int hp;
        public int Hp{ get=> hp;}
        private int currentHp;
        public int CurrentHp { get => currentHp; }
        private int atk;   
        public int Atk { get => atk; }

        public AbilityData(AbilityTressData data)
        {
            this.hp = data.hp;
            this.currentHp = data.hp;
            this.atk = data.atk;
        }
    }
}

