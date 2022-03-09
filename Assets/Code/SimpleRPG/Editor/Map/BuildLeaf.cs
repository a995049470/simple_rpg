using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    //只负责简单的实例化
    public class BuildLeaf : Leaf<BuildLeafData>
    {
        private GameObject currentPrefab;
        private Vector3Int spacing;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapMsgKind.MouseClick, Build);
        }

        private void Build(Msg msg)
        {
            if(currentPrefab == null) currentPrefab = data.DefalutPrefab;
            var msgData = msg.GetData<MouseClickData>();
            var position = msgData.mousePositionWS;
            if(data.Spacing.x != 0) position.x = Mathf.RoundToInt(position.x / data.Spacing.x) * data.Spacing.x;
            if(data.Spacing.y != 0) position.y = Mathf.RoundToInt(position.y / data.Spacing.y) * data.Spacing.y;
            if(data.Spacing.z != 0) position.z = Mathf.RoundToInt(position.z / data.Spacing.z) * data.Spacing.z;
            GameObject.Instantiate(currentPrefab, position, Quaternion.identity);
        }

    }

}

