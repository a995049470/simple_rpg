using System;

namespace SimpleRPG.Runtime
{
    [Flags]
    public enum UnitKind
    {
        None = 0,
        Player = 1 << 0,
        Enemy = 1 << 1,
    }
    public class UnitKindHelper
    {
        public static int GetUnitKinds(params UnitKind[] kinds)
        {
            int res = 0;
            foreach (var kind in kinds)
            {
                res = res | ((int)kind);
            }
            return res;
        }

        public static int AllKinds()
        {
            return -1;
        }
    }
}


