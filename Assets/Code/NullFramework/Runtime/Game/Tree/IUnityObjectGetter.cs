using UnityEngine;
namespace NullFramework.Runtime
{
    //获取gameObject的借口
    public interface IUnityObjectGetter
    {
        UnityEngine.Object GetUnityObject();
    }
}

