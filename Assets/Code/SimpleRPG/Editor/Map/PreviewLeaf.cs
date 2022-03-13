using NullFramework.Runtime;

namespace NullFramework.Editor
{
    public class PreviewLeaf : Leaf, ILeafReciver
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
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Preview);
        }
        private void Preview(Msg msg)
        {
            var msgData = msg.GetData<MapEditorEventData>();
            var e = msgData.currentEvent;
            if(e.type == UnityEngine.EventType.MouseMove)
            {
                mapData.ShowPreview(msgData.mouseWorldIntPosition);
            }
        }
    }

}

