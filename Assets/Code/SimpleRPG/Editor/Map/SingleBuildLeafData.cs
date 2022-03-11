using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    [CreateAssetMenu(fileName = "SingleBuildLeafData", menuName = "MapEditor/SingleBuildLeafData")]
    public class SingleBuildLeafData : LeafData<SingleBuildLeaf>, IFSMLeafData
    {
        public int GetLeafKind()
        {
            return ((int)OperateType.SingleBuild);
        }
    }

}

