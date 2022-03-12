using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    [CreateAssetMenu(fileName = "SingleDeleteLeafData", menuName = "MapEditor/SingleDeleteLeafData")]
    public class SingleDeleteLeafData : LeafData<SingleDeleteLeaf>, ILeafKindGetter
    {
        public int GetLeafKind()
        {
            return ((int)OperateType.SingleDelete);
        }
    }

}

