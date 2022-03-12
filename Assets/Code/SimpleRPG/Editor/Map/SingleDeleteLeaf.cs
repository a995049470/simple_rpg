using UnityEngine;
using NullFramework.Runtime;

namespace NullFramework.Editor
{
    public class SingleDeleteLeaf : Leaf, ILeafReciver
    {
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
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Delete);
        }

        private void Delete(Msg msg)
        {
            var msgData = msg.GetData<MapEditorEventData>();
            var e = msgData.currentEvent;
            if(e.isMouse && e.button == 1 && e.type == EventType.MouseDown && !e.alt && !e.control && !e.command)
            {
                mapData.TrayRmoveBuild(msgData.mouseWorldIntPosition);
            }
        }
    }

}

