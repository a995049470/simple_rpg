using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{

    //只负责简单的实例化
    public class SingleBuildLeaf : Leaf, ILeafReciver
    {
        private GameObject currentPrefab;
        private Vector3Int spacing;
        private MapData mapData;

        public void ReciveLeaf(Leaf leaf)
        {
            if(leaf is OperateManagerTress tress)
            {
                this.mapData = tress.CurrentMapData;
            }
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Build);
        }

        private void Build(Msg msg)
        {
            var msgData = msg.GetData<MapEditorEventData>();
            var e = msgData.currentEvent;
            if(e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                mapData.TrayAddBuild(msgData.mouseWorldIntPosition);
            }
        }

    }

}

