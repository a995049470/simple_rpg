using UnityEngine;

namespace NullFramework.Runtime
{
    public abstract class LeafData<T> : LeafData where T : Leaf, new()
    {
        public override Leaf CreateLeaf()
        {
            var leaf = new T();
            leaf.LoadData(this);
            leaf.AfterLoadData();
            return leaf;
        }
    }
    
    public abstract class LeafData : ScriptableObject 
    {
        public virtual Leaf CreateLeaf()
        {
           return default;
        }
    }
}
