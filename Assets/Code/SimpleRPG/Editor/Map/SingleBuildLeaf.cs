using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{

    //只负责简单的实例化
    public class SingleBuildLeaf : Leaf<SingleBuildLeafData>
    {
        private GameObject currentPrefab;
        private Vector3Int spacing;

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapMsgKind.MapEditorEvent, Build);
        }

        private void Build(Msg msg)
        {
            var msgData = msg.GetData<MapEditorEventData>();
            var e = msgData.currentEvent;
            if(e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                if(!currentPrefab) currentPrefab = leafData.DefalutPrefab;
                GameObject.Instantiate(currentPrefab, msgData.mousePositionWS, Quaternion.identity);
            }
        }

    }

}

