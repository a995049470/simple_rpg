using System.Collections.Generic;

namespace NullFramework.Runtime
{
    public class MsgData_CollectChilds : MsgData
    {
        public Tress tress;
        public List<Leaf> childs;
        public int depth = -1;

        public void AddChild(Leaf leaf)
        {
            if(depth == 0) return;
            childs.Add(leaf);
            depth --;
        }

    }
}
