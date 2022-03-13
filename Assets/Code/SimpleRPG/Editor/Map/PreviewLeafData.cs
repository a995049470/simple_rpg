using NullFramework.Runtime;
using UnityEngine;

namespace NullFramework.Editor
{
    [CreateAssetMenu(fileName = "PreviewLeafData", menuName = "MapEditor/PreviewLeafData")]
    public class PreviewLeafData : LeafData<PreviewLeaf>, ILeafKindGetter
    {
        public int GetLeafKind()
        {
            return ((int)OperateType.Preview);
        }
    }

}

