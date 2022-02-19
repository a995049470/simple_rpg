using UnityEngine;
namespace NullFramework.Runtime
{
    //获取gameObject的借口
    //TODO:后面考虑是否需要换数组或者字典
    public interface IUnityObjectGetter
    {
        UnityEngine.Object GetUnityObject();
    }
}

