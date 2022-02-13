using NullFramework.Runtime;
using UnityEngine;

namespace SimpleRPG.Runtime
{
    public class TreeManager : SingleMono<TreeManager>
    {
        Root root;
        private void OnEnable() {
            InitTree();
        }

        private void InitTree()
        {
            root = new Root_SimpleRPG();
            root.AddLeaf(0, new MoveLeaf(), true);
        }

        public void Update()
        {
            root.Update(Time.deltaTime);
        }
    }
}

