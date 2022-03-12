using UnityEngine;

namespace NullFramework.Runtime
{
    public abstract class LeafData<T> : LeafData where T : Leaf, new()
    {
        public override Leaf CreateLeaf()
        {
            var leaf = new T();
            leaf.LoadData(this);
            if(this is ILeafKindGetter kindLeaf)
            {
                leaf.SetLeafKind(kindLeaf.GetLeafKind());
            }
            return leaf;
        }
    }
    
    public abstract class LeafData : ScriptableObject 
    {
        public abstract Leaf CreateLeaf();
    }
}
