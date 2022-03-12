using System.Collections.Generic;

namespace NullFramework.Runtime
{
    //将字节点以kind做key 存在字典中
    public class ManagerTress : Tress
    {
        private Dictionary<int, Leaf> leafs;
        public ManagerTress()
        {
            leafs = new Dictionary<int, Leaf>();
        }

        public void AddLeaf(Leaf leaf)
        {
            leafs[leaf.Kind] = leaf;
        }

        public void RemoveLeaf(Leaf leaf)
        {
            leafs.Remove(leaf.Kind);
        }

        public override void AddChild(Leaf leaf, bool isActive)
        {
            AddLeaf(leaf);
            leaf.SetActive(isActive);
        }

        //设置叶节点的激活状态
        public void SetLeafActive(int kind, bool active)
        {
            if(leafs.TryGetValue(kind, out var leaf))
            {
                leaf.SetActive(active);
            }
        }
    }
}
