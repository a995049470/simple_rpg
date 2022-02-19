using UnityEngine;
namespace NullFramework.Runtime
{
    //设置gameObject的借口
    //TODO:后面考虑是否需要换数组或者字典
    public interface IUnityObjectSetter
    {
        void SetUnityObject(UnityEngine.Object obj);
    }
}

