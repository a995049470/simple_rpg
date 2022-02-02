namespace NullFramework.Runtime
{
    //TODO:对所有标识ID进行分层 提高效率
    public static partial class LeafKind
    {
        public const int Idle = 1;
        public const int Move = 2;
        public const int Execute = 3;
        public const int FSM = 4;

        public const int Player_Self = 5;
        public const int Player_Enemy = 6;
        public const int Player_Mid = 7;
    }
}
