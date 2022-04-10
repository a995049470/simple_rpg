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
            this.hp = data.Hp;
            this.currentHp = data.Hp;
            this.atk = data.Atk;
        }
    }
}

